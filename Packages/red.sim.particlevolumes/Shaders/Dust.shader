Shader "GPU Particle Volumes/Dust" {
    
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DrawDistance ("Draw distance", Float) = 20
        _Size ("Particle Size", Float)  = 0.01
        _SizeRandomize ("Particle Size Randomize", Range(0,1))  = 0.75
        _Amplitude ("Amplitude", Float) = 1
        _Frequency ("Frequency", Float) = 1
        _EdgeFade ("Edge Fade", Range(0,1)) = 0.015
        _VisibleAmount ("Visible Amount", Range(0,1)) = 1
        [Toggle(SNOW_USE_LIGHTVOLUMES)] _UseLightVolumes ("Use Light Volumes", Float) = 1
    }

    SubShader {

        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        LOD 100

        Pass {

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma shader_feature __ SNOW_USE_LIGHTVOLUMES

            #include "UnityCG.cginc"
            #include "LightVolumes.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _Color;

            float  _DrawDistance;
            float  _Size;
            float  _SizeRandomize;
            float  _Amplitude;
            float  _Frequency;
            float  _EdgeFade;
            float  _UseLightVolumes;
            float  _VisibleAmount;

            // Particle Volumes
            float4x4 _invWorldMatrix[128];
            uint _volumesCount;
            uint _volumesIncludersCount;

            // Particle culling macros
            #define CULL_VERTEX(o) { o.pos=float4(0,0,0,-1); o.uv=0; o.color=0; return o; }
            #define PI2 6.28318530718

            struct appdata {
                uint vertexID : SV_VertexID;// Vertex id for UV calculations
                float4 vertex : POSITION;   // vertex.xz - XZ default pos shift [-0.5..0.5], vertex.y - seed1 [0..1]
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos   : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv    : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Random X, Y, Z
                float y = v.vertex.y; // Random Y pos [0..1]
                float x = v.vertex.x; // Random X pos [0..1]
                float z = v.vertex.z; // Random Z pos [0..1]
                float rndXYZ = frac((z + y + x) * 17); // // Random using XYZ pos [0..1]

                // Cull particles to decrease snow density with Snowing parameter
                if (rndXYZ > _VisibleAmount) CULL_VERTEX(o);

                // Extra random value
                uint particleID = floor(v.vertexID / 4); // Particle ID is the same for each particle vertex
                float rnd = frac((float) 175993 / (particleID)); // Extra random value [0..1] - Used in time shift and X wobling

                float size = _DrawDistance; // Volume size
                float radius = size * 0.5; // Volume radius
                float time = _Time.y + rnd * 100; // Particle time shifted by random
                float3 camForward = - UNITY_MATRIX_V[2].xyz;
                float3 camPos = _WorldSpaceCameraPos + camForward * radius;

                // Movement
                float wobAmp  = _Amplitude; // Amplitude
                float wobFreq = max(_Frequency, 0.0); // Frequency

                float wobX = sin(time * wobFreq + rnd * PI2) * wobAmp; // X wobbling
                float wobY = sin(time * wobFreq + x * PI2) * wobAmp; // Y wobbling
                float wobZ = sin(time * wobFreq + y * PI2) * wobAmp; // Z wobbling

                float3 preTileXYZ = float3(x, y, z) * size + float3(wobX, wobY, wobZ); // XYZ position before tiling, using vertex.xyz as XZ default pos shift [-0.5..0.5]

                // Tiling to [-R..R] cube
                float3 tileSize = float3(size, size, size);
                float3 tilesXYZ = floor((preTileXYZ - camPos.xyz) / tileSize + 0.5); // Tiling
                float3 worldPos = preTileXYZ - tilesXYZ * tileSize; // Final World Position

                // Culling if particle is behind the camera
                float3 toCam = worldPos - _WorldSpaceCameraPos; // From particle to camera vector
                float  zView = dot(toCam, camForward);
                if (zView <= 0.0) CULL_VERTEX(o); 

                // Volumes culling
                uint includersCount = min(_volumesIncludersCount, 128);
                uint count = min(_volumesCount, 128);
                bool isVisible = includersCount == 0; // If no includers - all the world is includer
                [loop] for(uint i = 0; i < includersCount; i++) {
                    if(all(abs(mul(_invWorldMatrix[i], float4(worldPos, 1.0)).xyz) <= 0.5)){ // If particle inside includer
                        isVisible = true;
                        break;
                    }
                }
                if(!isVisible) CULL_VERTEX(o);
                [loop] for(uint j = includersCount; j < count; j++) {
                    if(all(abs(mul(_invWorldMatrix[j], float4(worldPos, 1.0)).xyz) <= 0.5)){ // If particle inside excluder
                        CULL_VERTEX(o);
                    }
                }

                // Edge fade
                float edgeK = 1.0;
                if (_EdgeFade > 0.0001) {
                    float inner = radius * (1.0 - _EdgeFade);
                    float fadeLen = max(radius - inner, 1e-4);
                    float3 absLocal = abs(worldPos - camPos + camForward * radius * _EdgeFade);
                    float  distToFace = radius - max(absLocal.x, max(absLocal.y, absLocal.z));
                    edgeK = saturate(distToFace / fadeLen);
                }

                float pSize = _Size * (1 + _SizeRandomize * (2 * rnd - 1)) * edgeK; // Final particle size
                float rWorld = pSize * 1.41421356; // Half of the quad diagonal

                // Frustum culling
                float4 c = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0)); // Clip space center
                float pad_clip = rWorld * abs(UNITY_MATRIX_P[0][0]) * c.w / zView; // Clip space padding
                if (c.x < -c.w - pad_clip || c.x > c.w + pad_clip || c.y < -c.w - pad_clip || c.y > c.w + pad_clip || c.z <  0.0 || c.z > c.w + rWorld) CULL_VERTEX(o);

                // Coloring with Light Volumes
                fixed4 tint = _Color;
                #ifdef SNOW_USE_LIGHTVOLUMES
                    tint *= _UdonLightVolumeEnabled > 0 ? float4(LightVolumeSH_L0(worldPos), 1) : 1; // Coloring based on Light Volumes
                #endif

                // Forming UV
                float2 uv = float2(v.vertexID & 1, (v.vertexID >> 1) & 1);

                // Random particle flip
                uint r = fmod(particleID, 4); // Flip id
                float2 signs = 1.0 - float2(r >> 1, (r ^ (r >> 1)) & 1) * 2.0;

                // Creating a particle billboard
                float2 quad = (uv.xy * 2.0 - 1.0) * pSize; // use uv.xy as quad UV
                float3 right = UNITY_MATRIX_V[0].xyz * signs.x;
                float3 up    = UNITY_MATRIX_V[1].xyz * signs.y;

                // Moving vertex positions to form a quad shape
                worldPos += right * quad.x + up * quad.y;

                // Sending to fragment shader
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(uv.xy, _MainTex); // pass uv.xy to fragment
                o.color = tint;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}