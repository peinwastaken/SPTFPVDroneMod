using UnityEngine;

#if !UNITY_EDITOR
using FPVDroneMod.Helpers;
#endif

namespace FPVDroneMod.Components
{
    public class DroneSoundController : MonoBehaviour
    {
        public float VolumeMin = 0.1f;
        public float VolumeMax = 0.8f;
        public float PitchMin = 0.9f;
        public float PitchMax = 1.5f;

        public AudioClip DroneSound;
        public AudioSource AudioSource;
        public DroneController DroneController;
        
        #if !UNITY_EDITOR
        public void SetBlend(float pos)
        {
            DroneController = GetComponent<DroneController>();
        }

        private void Start()
        {
            DroneSound = AssetHelper.DroneAudioClip;
            DroneController = GetComponent<DroneController>();
            
            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = DroneSound;
            AudioSource.loop = true;
            AudioSource.volume = 0.2f;
            AudioSource.minDistance = 1f;
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
