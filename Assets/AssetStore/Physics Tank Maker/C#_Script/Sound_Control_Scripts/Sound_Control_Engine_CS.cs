using UnityEngine;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(AudioSource))]

    public class Sound_Control_Engine_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Engine_Sound" object in the tank.
		 * This script controls the engine sound in the tank.
		*/

        // User options >>
        public float Min_Engine_Pitch = 1.0f;
        public float Max_Engine_Pitch = 2.0f;
        public float Min_Engine_Volume = 0.5f;
        public float Max_Engine_Volume = 1.0f;
        public Rigidbody Left_Reference_Rigidbody;
        public Rigidbody Right_Reference_Rigidbody;
        public string Reference_Name_L;
        public string Reference_Name_R;
        public string Reference_Parent_Name_L;
        public string Reference_Parent_Name_R;
        // << User options

        // For the editor script to display the current velocity.
        public float Left_Velocity;
        public float Right_Velocity;

        // For editor script.
        public bool Has_Changed;

        AudioSource thisAudioSource;
        float currentRate;
        float targetRate;
        float accelerationRate = 8.0f;
        float decelerationRate = 4.0f;
        Drive_Control_CS driveScript;


        void Start()
        {
            Initial_Settings();
        }


        void Initial_Settings()
        {
            thisAudioSource = GetComponent<AudioSource>();
            thisAudioSource.playOnAwake = false;
            thisAudioSource.loop = true;
            thisAudioSource.volume = 0.0f;
            // thisAudioSource.Play(); Moved to SoundControl script

            // Find the reference rigidbodies.
            Transform bodyTransform = transform.parent;
            if (Left_Reference_Rigidbody == null)
            { // The left reference wheel has been lost by modifying.
                if (string.IsNullOrEmpty(Reference_Name_L) == false && string.IsNullOrEmpty(Reference_Parent_Name_L) == false)
                {
                    Transform leftReferenceTransform = bodyTransform.Find(Reference_Parent_Name_L + "/" + Reference_Name_L);
                    if (leftReferenceTransform)
                    {
                        Left_Reference_Rigidbody = leftReferenceTransform.GetComponent<Rigidbody>();
                    }
                }
            }
            if (Right_Reference_Rigidbody == null)
            { // The right reference wheel has been lost by modifying.
                if (string.IsNullOrEmpty(Reference_Name_R) == false && string.IsNullOrEmpty(Reference_Parent_Name_R) == false)
                {
                    Transform rightReferenceTransform = bodyTransform.Find(Reference_Parent_Name_R + "/" + Reference_Name_R);
                    if (rightReferenceTransform)
                    {
                        Right_Reference_Rigidbody = rightReferenceTransform.GetComponent<Rigidbody>();
                    }
                }
            }
            if (Left_Reference_Rigidbody == null || Right_Reference_Rigidbody == null)
            {
                Debug.LogWarning("'Reference Rigidbody' for the engine sound cannot be found.");
                Destroy(this);
            }

            // Get the "Drive_Control_CS".
            driveScript = GetComponentInParent<Drive_Control_CS>();
        }


        void Update()
        {
            Engine_Sound();
        }


        void Engine_Sound()
        {
            // Get the velocity.
            Left_Velocity = Left_Reference_Rigidbody.velocity.magnitude;
            Right_Velocity = Right_Reference_Rigidbody.velocity.magnitude;

            // Set the rate.
            targetRate = (Left_Velocity + Right_Velocity) / 2.0f / driveScript.Max_Speed;

            if (targetRate > currentRate)
            { // Acceleration
                currentRate = Mathf.MoveTowards(currentRate, targetRate, accelerationRate * Time.deltaTime);
            }
            else
            { // Deceleration
                currentRate = Mathf.MoveTowards(currentRate, targetRate, decelerationRate * Time.deltaTime);
            }
            // Set the pitch and volume.
            thisAudioSource.pitch = Mathf.Lerp(Min_Engine_Pitch, Max_Engine_Pitch, currentRate);
            thisAudioSource.volume = Mathf.Lerp(Min_Engine_Volume, Max_Engine_Volume, currentRate);
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            thisAudioSource.Stop();
            Destroy(this);
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".
            this.enabled = !isPaused;
            if (isPaused)
            {
                thisAudioSource.Stop();
            }
            else
            {
                thisAudioSource.Play();
            }
        }

    }

}