using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

    public class Particle_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the explosion effects prefabs.
		 * This script controls the sound and the light in the effects.
		*/


        // User options >>
        public ParticleSystem This_ParticleSystem;
        public bool Use_Random_Pitch;
        public float Random_Pitch_Min = 0.5f;
        public float Random_Pitch_Max = 1.0f;
        // << User options


        Light thisLight;
        AudioSource audioSource;


        void Start()
        {
            // Get the ParticleSystem.
            if (This_ParticleSystem == null)
            {
                This_ParticleSystem = GetComponent<ParticleSystem>();
            }

            // Get the Light.
            thisLight = GetComponent<Light>();
            if (thisLight)
            {
                StartCoroutine("Flash");
            }

            // Get the AudioSource.
            audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                // Setup the AudioSource.
                audioSource.playOnAwake = false;
                var mainCamera = Camera.main;
                if (mainCamera)
                {
                    // Get the distance to the camera.
                    var dist = Vector3.Distance(transform.position, mainCamera.transform.position);
                    if (Use_Random_Pitch)
                    {
                        // Change the pitch randomly.
                        audioSource.pitch = Random.Range(Random_Pitch_Min, Random_Pitch_Max);
                    }
                    else
                    {
                        // Change the pitch according to the distance to the camera. 
                        audioSource.pitch = Mathf.Lerp(1.0f, 0.1f, dist / audioSource.maxDistance);
                    }

                    // Delay playing the sound according to the distance to the camera. 
                    audioSource.PlayDelayed(dist / 340.29f * Time.timeScale);
                }
            }
        }


        void Update()
        {
            // Check the particles and the audio have finished.
            if (This_ParticleSystem.isStopped)
            { // The particle has finished.
                if (audioSource && audioSource.isPlaying)
                { // The audio is still playing now.
                    return;
                }
                // The audio has finished.

                Destroy(this.gameObject);
            }
        }


        IEnumerator Flash()
        {
            thisLight.enabled = true;
            yield return new WaitForSeconds(0.08f);
            thisLight.enabled = false;
        }

    }

}