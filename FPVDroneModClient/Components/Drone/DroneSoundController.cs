using FPVDroneModClient.Components.Base;
using UnityEngine;
#if !UNITY_EDITOR
using Audio.NPC;
using Comfort.Common;
using FPVDroneModClient.Helpers;
#endif

namespace FPVDroneModClient.Components.Drone
{
    public class DroneSoundController : MonoBehaviour
    {
        public float VolumeMin = 0.1f;
        public float VolumeMax = 0.8f;
        public float PitchMin = 0.9f;
        public float PitchMax = 1.5f;

        public AudioClip DroneSound;
        public AudioSource AudioSource;
        public BaseDroneController DroneController;

        #if !UNITY_EDITOR
        public void SetBlend(float pos)
        {
            DroneController = GetComponent<BaseDroneController>();
        }

        private void Start()
        {
            DroneController = GetComponent<BaseDroneController>();

            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = DroneSound;
            AudioSource.loop = true;
            AudioSource.volume = 0.2f;
            AudioSource.minDistance = 1f;
            AudioSource.maxDistance = 150f;
            AudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            AudioSource.spatialBlend = 0f;
            AudioSource.playOnAwake = false;
            AudioSource.Stop();
        }

        private void Update()
        {
            if (AudioSource)
            {
                float volume = Mathf.Lerp(VolumeMin, VolumeMax, DroneController.Thrust);
                float pitch = Mathf.Lerp(PitchMin, PitchMax, DroneController.Thrust);
                AudioSource.volume = volume;
                AudioSource.pitch = pitch;
            }
        }
        #endif
    }
}
