using UnityEngine;

namespace ChobiAssets.PTM
{

    [RequireComponent(typeof(AudioSource))]

    public class Sound_Control_Motor_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Turret_Base" or "Cannon_Base" in the turret.
		 * This script controls the rotation sound of the turret of cannon.
		 * This script works in combination with "Turret_Horizontal_CS" or "Cannon_Vertical_CS" in the turret.
		*/


        // User options >>
        public float Max_Motor_Volume = 1.0f;
        // << User options


        Turret_Horizontal_CS turretScript;
        Cannon_Vertical_CS cannonScript;
        int type; // 0 = Turret, 1 = Cannon.
        float targetVolume;

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
            thisAudioSource.loop = true;
            thisAudioSource.volume = 0.0f;
            thisAudioSource.Play();

            // Find the reference script, set the type.
            turretScript = GetComponent<Turret_Horizontal_CS>();
            if (turretScript is not null)
            {
                type = 0; // Turret
                return;
            }
            cannonScript = GetComponent<Cannon_Vertical_CS>();
            if (cannonScript is not null)
            {
                type = 1; // Cannon
                return;
            }

            // The reference script cannot be found.
            Debug.LogWarning("The reference script for the 'Motor Sound' cannot be found.");
            Destroy(this);
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            Set_Volume();
        }


        void Set_Volume()
        {
            switch (type)
            {
                case 0: // Turret
                    targetVolume = Mathf.Lerp(0.0f, Max_Motor_Volume, Mathf.Abs(turretScript.Turn_Rate));
                    break;

                case 1: // Cannon
                    targetVolume = Mathf.Lerp(0.0f, Max_Motor_Volume, Mathf.Abs(cannonScript.Turn_Rate));
                    break;
            }

            thisAudioSource.volume = Mathf.MoveTowards(thisAudioSource.volume, targetVolume, 0.8f * Time.fixedDeltaTime);
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            thisAudioSource.Stop();
            Destroy(this);
        }


        void Turret_Destroyed_Linkage()
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