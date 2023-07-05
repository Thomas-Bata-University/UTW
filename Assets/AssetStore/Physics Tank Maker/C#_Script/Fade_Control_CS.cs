using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

    public class Fade_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the image used for 'fade in' and 'fade out'.
		 * The 'fade in' is called at the opening of the scene.
		 * The 'fade out' is called from "Scene_Open_CS" in the buttons before the scene has been changed.
		*/


        // User options >>
        public Image Fade_Image;
        public float Fade_Time = 1.0f;
        // << User options


        public static Fade_Control_CS Instance;


        void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            if (Fade_Image == null)
            {
                Fade_Image = GetComponent<Image>();
            }

            // Fade in at the start of the scene.
            StartCoroutine("Fade_In");
        }


        IEnumerator Fade_In()
        {
            float count = 0.0f;
            Color currentColor = Fade_Image.color;
            while (count < Fade_Time)
            {
                // Increase the audio volume.
                AudioListener.volume = count / Fade_Time;

                // Decrease the image alpha.
                currentColor.a = 1.0f - (count / Fade_Time);
                Fade_Image.color = currentColor;

                count += Time.unscaledDeltaTime; ;
                yield return null;
            }

            AudioListener.volume = 1.0f;
            currentColor.a = 0.0f;
            Fade_Image.color = currentColor;
        }


        public IEnumerator Fade_Out()
        { // Called from "Scene_Open_CS" attached to buttons in the scene.
            float count = 0.0f;
            Color currentColor = Fade_Image.color;
            while (count < Fade_Time)
            {
                // Decrease the audio volume.
                AudioListener.volume = 1.0f - (count / Fade_Time);

                // Increase the image alpha.
                currentColor.a = count / Fade_Time;
                Fade_Image.color = currentColor;

                count += Time.unscaledDeltaTime; ;
                yield return null;
            }
        }



    }

}
