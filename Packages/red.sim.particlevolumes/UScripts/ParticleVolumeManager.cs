using UdonSharp;
using UnityEngine;

namespace GPUParticleVolumes {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleVolumeManager : UdonSharpBehaviour {

        public MeshRenderer MeshRenderer;
        public Transform[] ParticleVolumesIncluders;
        public Transform[] ParticleVolumesExcluders;
        public bool AutoUpdateVolumes = false;

        private Matrix4x4[] _matrices = new Matrix4x4[128];

        private void Start() {
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

            // Setting variables to material
            MeshRenderer.sharedMaterial.SetMatrixArray("_invWorldMatrix", _matrices);
            MeshRenderer.sharedMaterial.SetInteger("_volumesCount", count);
            MeshRenderer.sharedMaterial.SetInteger("_volumesIncludersCount", includersCount);

        }

    }
}