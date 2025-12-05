#if UDONSHARP
using UdonSharp;
#endif
using UnityEngine;

namespace GPUParticleVolumes {

#if UDONSHARP
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleVolumeManager : UdonSharpBehaviour {
#else
    public class ParticleVolumeManager : MonoBehaviour {
#endif
        public MeshRenderer MeshRenderer;
        public Transform[] ParticleVolumesIncluders;
        public Transform[] ParticleVolumesExcluders;
        public bool AutoUpdateVolumes = false;

        private Matrix4x4[] _matrices = new Matrix4x4[128];
        private MaterialPropertyBlock _materialProperty;

        private void Start() {
            _materialProperty = new MaterialPropertyBlock();
            UpdateLoop();
        }

        public void UpdateLoop() {
            UpdateVolumes();
            if (AutoUpdateVolumes) SendCustomEventDelayedFrames("UpdateLoop", 1, VRC.Udon.Common.Enums.EventTiming.Update);
        }

        public void UpdateVolumes() {
            if (MeshRenderer == null || MeshRenderer.sharedMaterial == null) return;
#if UNITY_EDITOR
            _matrices = new Matrix4x4[128];
            _materialProperty = new MaterialPropertyBlock();
#endif
            int count = 0; // All volumes count
            for (int i = 0; i < ParticleVolumesIncluders.Length; i++) {
                Transform volume = ParticleVolumesIncluders[i];
                if (volume == null || !volume.gameObject.activeInHierarchy) continue;
                _matrices[count] = Matrix4x4.TRS(volume.position, volume.rotation, volume.lossyScale).inverse;
                count++;
                if (count >= 128) break;
            }
            int includersCount = count; // Includer volumes count
            for (int i = 0; i < ParticleVolumesExcluders.Length; i++) {
                Transform volume = ParticleVolumesExcluders[i];
                if (volume == null || !volume.gameObject.activeInHierarchy) continue;
                _matrices[count] = Matrix4x4.TRS(volume.position, volume.rotation, volume.lossyScale).inverse;
                count++;
                if (count >= 128) break;
            }

            // Setting variables to material property block
            _materialProperty.SetMatrixArray("_invWorldMatrix", _matrices);
            _materialProperty.SetInteger("_volumesCount", count);
            _materialProperty.SetInteger("_volumesIncludersCount", includersCount);
            MeshRenderer.SetPropertyBlock(_materialProperty);

        }

    }
}