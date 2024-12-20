using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

    [System.Serializable]
    class TextProp
    {
        public string textString;
        public Color textColor;
        public float displayTime;

        public TextProp(string newString, Color newColor, float newTime)
        {
            textString = newString;
            textColor = newColor;
            displayTime = newTime;
        }
    }

    public class UI_Text_Control_CS : MonoBehaviour
    {
        /*
		 * This script is attached to a Text in the scene, and displays the messages sent from the "Event_Event_02_Show_Message_CS" script in the scene.
		*/


        // User options >>
        public float Fade_In_Time = 1.0f;
        public float Fade_Out_Time = 1.0f;
        // << User options


        Text thisText;
        List<TextProp> textList = new List<TextProp>();
        bool isDisplaying = false;
        bool isWaiting = false;


        void Start()
        {
            thisText = GetComponent<Text>();
            thisText.text = null;
        }


        public void Receive_Text(string tempString, Color currentColor, float tempTime)
        { // Called from "Event_Show_Message" in the scene.

            // Add the text information into the list.
            TextProp newTextProp = new TextProp(tempString, currentColor, tempTime);
            textList.Add(newTextProp);

            if (isDisplaying)
            { // Other text is displaying now.
                isWaiting = true;
            }
        }


        void Update()
        {
            if (textList.Count > 0 && isDisplaying == false)
            { // The list has any text informations, and this script is not displaying any text now.
                // Start displaying the next text.
                thisText.text = textList[0].textString;
                thisText.color = textList[0].textColor;
                StartCoroutine("Fade_In");
            }
        }


        IEnumerator Fade_In()
        {
            isDisplaying = true;

            var count = 0.0f;
            var currentColor = thisText.color;
            var targetAlpha = currentColor.a;
            while (count < Fade_In_Time)
            {
                currentColor.a = Mathf.Lerp(0.0f, targetAlpha, count / Fade_In_Time);
                thisText.color = currentColor;
                count += Time.deltaTime;
                yield return null;
            }

            currentColor.a = targetAlpha;
            thisText.color = currentColor;

            StartCoroutine("Keep_Displaying");
        }


        IEnumerator Keep_Displaying()
        {
            var count = 0.0f;
            while (count < textList[0].displayTime)
            {
                if (isWaiting)
                { // Other text is waiting now.
                    isWaiting = false;
                    break; // Skip displaying, and go to "Fade_Out()".
                }
                else
                {
                    count += Time.deltaTime;
                    yield return null;
                }
            }

            StartCoroutine("Fade_Out");
        }


        IEnumerator Fade_Out()
        {

            var count = 0.0f;
            var currentColor = thisText.color;
            var initialAlpha = currentColor.a;
            while (count < Fade_Out_Time)
            {
                currentColor.a = Mathf.Lerp(initialAlpha, 0.0f, count / Fade_Out_Time);
                thisText.color = currentColor;
                count += Time.deltaTime;
                yield return null;
            }

            currentColor.a = 0.0f;
            thisText.color = currentColor;

            // Remove the text information from the list.
            textList.RemoveAt(0);

            isDisplaying = false;
        }

    }

}