using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(AudioSource))]

    public class Sound_Control_Impact_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the MainBody in the tank.
		 * This script controls the impact sound of the tank.
		*/


        // User options >>
        [SerializeField] float Min_Variation = 0.01f;
        [SerializeField] float Max_Variation = 0.05f;
        [SerializeField] float Min_Pitch = 0.5f;
        [SerializeField] float Max_Pitch = 1.0f;
        [SerializeField] float Min_Volume = 0.5f;
        [SerializeField] float Max_Volume = 1.0f;
        [SerializeField] float Interval = 0.1f;
        // << User options


        float currentVelocity;
        float previousVelocity;
        bool isPrepared = true;
        Rigidbody bodyRigidbody;
        AudioSource thisAudioSource;

        bool isSelected;


        void Start()
        {
            Initial_Settings();
        }


        void Initial_Settings()
        {
            thisAudioSource = GetComponent<AudioSource>();
            thisAudioSource.playOnAwake = false;
            thisAudioSource.loop = false;
            bodyRigidbody = GetComponent<Rigidbody>();
        }


        void FixedUpdate()
        {
            if (isSelected)
            {
                Impact_Sound();
            }
        }


        void Impact_Sound()
        {
            // Get the current variation.
            currentVelocity = bodyRigidbody.velocity.y * Time.fixedDeltaTime;
            float currentVariation = Mathf.Abs(previousVelocity - currentVelocity);

            // Check the variation.
            if (isPrepared && currentVariation > Min_Variation)
            {
                // Produce the sound.
                StartCoroutine("Produce_Impact_Sound", currentVariation / Max_Variation);
            }

            // Store the current variation.
            previousVelocity = currentVelocity;
        }


        IEnumerator Produce_Impact_Sound(float rate)
        {
            isPrepared = false;
            thisAudioSource.pitch = Mathf.Lerp(Min_Pitch, Max_Pitch, rate);
            thisAudioSource.volume = Mathf.Lerp(Min_Volume, Max_Volume, rate);
            thisAudioSource.PlayOneShot(thisAudioSource.clip);
            yield return new WaitForSeconds(Interval);
            isPrepared = true;
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            thisAudioSource.Stop();
            Destroy(this);
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.isSelected = isSelected;
        }

    }

}