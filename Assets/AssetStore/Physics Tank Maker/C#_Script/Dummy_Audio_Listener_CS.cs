using UnityEngine;


namespace ChobiAssets.PTM
{

    /*
	* This script is attached to "Game_Controller" in the scene.
	* This script is used only for removing the dummy AudioListener set as the defualt AudioListener.
    */


    public class Dummy_Audio_Listener_CS : MonoBehaviour
    {

        AudioListener thisAudioListener;


        void Awake()
        {
            thisAudioListener = GetComponent<AudioListener>();
            if (thisAudioListener == null)
            {
                Destroy(this);
                return;
            }

            var audioListeners = FindObjectsOfType<AudioListener>();
            if (audioListeners.Length > 1)
            { // There is already any tank instance in the scene.
                Destroy(thisAudioListener);
                Destroy(this);
            }
        }


        void Update()
        {
            Destroy(thisAudioListener);
            Destroy(this);
        }

    }

}
