using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GPUParticleVolumes {
    public class ParticleVolumeMeshGeneratorWindow : EditorWindow {
        private int particleCount = 100000;
        private const int MaxParticlesByArraySize = 100000000;

        private static readonly Vector2 DefaultSize = new Vector2(420, 100);

        [MenuItem("Tools/GPU Particle Volumes/Simple Particle Mesh Generator")]
        public static void ShowWindow() {
            var window = GetWindow<ParticleVolumeMeshGeneratorWindow>(
                false,
                "Simple Particle Mesh Generator",
                true
            );

            // Set initial size but allow resize
            window.position = GetCenteredRect(DefaultSize);

            // No forced fixed size → Unity default resizable window
            window.minSize = new Vector2(300, 120); // Just minimal sane size

            window.Show();
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("Simple Particle Volume Mesh Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            particleCount = EditorGUILayout.IntField("Particle Count", particleCount);
            if (EditorGUI.EndChangeCheck()) {
                if (particleCount < 1) particleCount = 1;
                if (particleCount > MaxParticlesByArraySize) particleCount = MaxParticlesByArraySize;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Mesh and Save...", GUILayout.Height(28))) {
                GenerateAndSave();
            }
        }

        private void GenerateAndSave() {
            string defaultName = $"GpuParticles_{particleCount}.asset";

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Particle Mesh",
                defaultName,
                "asset",
                "Choose a location to save the Mesh asset."
            );

            if (string.IsNullOrEmpty(path))
                return;

            try {
                EditorUtility.DisplayProgressBar(
                    "Generating Particle Mesh",
                    "Preparing data...",
                    0f
                );

                Mesh mesh = BuildMeshWithProgress(particleCount);

                var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                if (existing != null)
                    AssetDatabase.DeleteAsset(path);

                AssetDatabase.CreateAsset(mesh, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Done", $"Mesh has been saved:\n{path}", "OK");
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }

        private static Mesh BuildMeshWithProgress(int particleCount) {
            var mesh = new Mesh();
            mesh.name = $"GpuParticles_{particleCount}";

            int vCount = particleCount * 4;
            int iCount = particleCount * 6;

            if (vCount > 65535)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            var verts = new Vector3[vCount];
            var tris = new int[iCount];

            const int step = 10000;
            for (int i = 0; i < particleCount; i++) {
                if ((i % step) == 0) {
                    EditorUtility.DisplayProgressBar(
                        "Generating Particles Mesh",
                        $"Generating particles: {i}/{particleCount}",
                        (float)i / particleCount
                    );
                }

                int v = i * 4;
                int t = i * 6;

                Vector3 p = new Vector3(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
                );

                verts[v + 0] = p;
                verts[v + 1] = p;
                verts[v + 2] = p;
                verts[v + 3] = p;

                tris[t + 0] = v + 0;
                tris[t + 1] = v + 1;
                tris[t + 2] = v + 2;
                tris[t + 3] = v + 2;
                tris[t + 4] = v + 1;
                tris[t + 5] = v + 3;
            }

            EditorUtility.DisplayProgressBar(
                "Generating Particles Mesh",
                "Finalizing mesh...",
                1f
            );

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000));

            return mesh;
        }

        //
        // Utility: center editor window
        //
        private static Rect GetCenteredRect(Vector2 size) {
            Rect main = GetEditorMainWindowRect();
            float w = size.x;
            float h = size.y;

            return new Rect(
                main.x + (main.width - w) * 0.5f,
                main.y + (main.height - h) * 0.5f,
                w,
                h
            );
        }

        // Reflectively get the main editor window rect
        private static Rect GetEditorMainWindowRect() {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.ContainerWindow");
            var showModeField = type?.GetField("m_ShowMode",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var positionProp = type?.GetProperty("position",
                BindingFlags.Public | BindingFlags.Instance);

            if (type == null || showModeField == null || positionProp == null)
                return new Rect(0, 0, 800, 600);

            var windows = Resources.FindObjectsOfTypeAll(type);
            foreach (var win in windows) {
                int showMode = (int)showModeField.GetValue(win);
                if (showMode == 4) // main editor window
                    return (Rect)positionProp.GetValue(win, null);
            }

            return new Rect(0, 0, 800, 600);
        }
    }
}