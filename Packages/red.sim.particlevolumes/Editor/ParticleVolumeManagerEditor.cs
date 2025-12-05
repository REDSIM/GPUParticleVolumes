using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace GPUParticleVolumes {
    [InitializeOnLoad]
    public class ParticleVolumeManagerEditor {

        private static ParticleVolumeManager[] _particleVolumeManagers;
        private static bool _isInitialized = false;

        static ParticleVolumeManagerEditor() {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.delayCall += RefreshManagersListAndInitialize;
            EditorApplication.hierarchyChanged += RefreshManagersList;
            EditorSceneManager.sceneSaved += RefreshManagersListAndInitialize;
        }

        // Searching for ParticleVolumeManagers
        private static void RefreshManagersListAndInitialize(Scene scene) {
            RefreshManagersList();
            _isInitialized = false;
        }
        private static void RefreshManagersListAndInitialize() {
            RefreshManagersList();
            _isInitialized = false;
        }
        private static void RefreshManagersList() {
            _particleVolumeManagers = Object.FindObjectsOfType<ParticleVolumeManager>();
        }

        // Drowing bounds volume
        private static void DrawVolumeBounds(Transform volume, Color color) {
            Handles.matrix = Matrix4x4.TRS(volume.position, volume.rotation, volume.lossyScale);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = color;
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            Handles.color = new Color(color.r, color.g, color.b, 0.2f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
            Handles.matrix = Matrix4x4.identity;
        }

        private static void OnSceneGUI(SceneView sceneView) {

            // Skip when no objects selected
            var go = Selection.activeGameObject;
            if (go == null && _isInitialized) return;

            var managers = _particleVolumeManagers;
            if (managers == null || managers.Length == 0) return;

            List<Transform> volumes = new List<Transform>();

            // Checking if particle volumes are selected
            int includersCount = 0;
            for (int i = 0; i < managers.Length; i++) {
                if (managers[i].ParticleVolumesIncluders != null) {
                    for (int j = 0; j < managers[i].ParticleVolumesIncluders.Length; j++) {
                        if (managers[i].ParticleVolumesIncluders[j] != null && Selection.Contains(managers[i].ParticleVolumesIncluders[j].gameObject)) {
                            volumes.Add(managers[i].ParticleVolumesIncluders[j]);
                            includersCount++;
                        }
                    }
                }
                if (managers[i].ParticleVolumesExcluders != null) {
                    for (int j = 0; j < managers[i].ParticleVolumesExcluders.Length; j++) {
                        if (managers[i].ParticleVolumesExcluders[j] != null && Selection.Contains(managers[i].ParticleVolumesExcluders[j].gameObject)) {
                            volumes.Add(managers[i].ParticleVolumesExcluders[j]);
                        }
                    }
                }
                managers[i].UpdateVolumes();
            }

            _isInitialized = true;

            // If more than one selected, skipping drawing handles
            if (volumes.Count == 0) return;

            // Drawing volumes bounds
            for (int i = 0; i < volumes.Count; i++) {
                DrawVolumeBounds(volumes[i], i < includersCount ? Color.white : Color.red);
            }

            if (volumes.Count > 1) return;

            Transform transform = volumes[0];

            // Volume transform
            var position = transform.position;
            var rotation = transform.rotation;
            var scale = transform.lossyScale;

            // Axis colors
            Color colorX = Handles.xAxisColor;
            Color colorY = Handles.yAxisColor;
            Color colorZ = Handles.zAxisColor;

            for (int i = 0; i < 6; i++) {

                Vector3 worldDirection = Vector3.zero;
                Vector3 worldUpDirection = Vector3.zero;

                int axisIndex = i / 2;
                bool isPositive = (i % 2 == 0);

                switch (axisIndex) {
                    case 0: // X
                        worldDirection = rotation * (isPositive ? Vector3.right : Vector3.left);
                        worldUpDirection = rotation * Vector3.up;
                        Handles.color = colorX;
                        break;
                    case 1: // Y
                        worldDirection = rotation * (isPositive ? Vector3.up : Vector3.down);
                        worldUpDirection = rotation * Vector3.right;
                        Handles.color = colorY;
                        break;
                    case 2: // Z
                        worldDirection = rotation * (isPositive ? Vector3.forward : Vector3.back);
                        worldUpDirection = rotation * Vector3.up;
                        Handles.color = colorZ;
                        break;
                }

                // Handle parameters
                Vector3 handlePos = position + worldDirection * scale[axisIndex] * 0.5f;
                float handleSize = HandleUtility.GetHandleSize(handlePos) * 0.2f;
                Vector3 handleOffset = handleSize * worldDirection * 0.1f / 0.2f;

                EditorGUI.BeginChangeCheck();

                // Drawing Cone handle
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
                Vector3 newHandleLocalPos = Handles.Slider(handlePos + handleOffset, worldDirection, handleSize, Handles.ConeHandleCap, 0.25f) - handleOffset;

                // Drawing X-Ray square
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawSolidRectangleWithOutline(GetPlaneVertices(handlePos, Quaternion.LookRotation(worldDirection, worldUpDirection), handleSize), new Color(1, 1, 1, 0.15f), Color.white);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                Handles.DrawSolidRectangleWithOutline(GetPlaneVertices(handlePos, Quaternion.LookRotation(worldDirection, worldUpDirection), handleSize), Color.clear, new Color(1, 1, 1, 0.25f));

                // Applying position and rotation
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(transform, "Scale Bounds Size");
                    float delta = Vector3.Dot(newHandleLocalPos - handlePos, worldDirection);
                    Vector3 modifiedScale = scale;
                    modifiedScale[axisIndex] += delta;
                    transform.position += worldDirection * delta / 2;
                    SetLossyScale(transform, modifiedScale);
                }
            }

        }

        // Plane vertices for drawing a square
        public static Vector3[] GetPlaneVertices(Vector3 center, Quaternion rotation, float size) {
            Vector3 right = rotation * Vector3.right * size;
            Vector3 up = rotation * Vector3.up * size;
            return new Vector3[] { center - right - up, center - right + up, center + right + up, center + right - up };
        }

        // Setting lossy scale to a specified transform
        public static void SetLossyScale(Transform transform, Vector3 targetLossyScale, int maxIterations = 20) {
            Vector3 guess = transform.localScale;
            for (int i = 0; i < maxIterations; i++) {
                transform.localScale = guess;
                Vector3 currentLossy = transform.lossyScale;
                Vector3 ratio = new Vector3(
                    currentLossy.x != 0 ? targetLossyScale.x / currentLossy.x : 1f,
                    currentLossy.y != 0 ? targetLossyScale.y / currentLossy.y : 1f,
                    currentLossy.z != 0 ? targetLossyScale.z / currentLossy.z : 1f
                );
                guess = new Vector3(guess.x * ratio.x, guess.y * ratio.y, guess.z * ratio.z);
            }
        }

    }
}