using UnityEngine;

namespace ChobiAssets.PTM
{

    [System.Serializable]
    public class Tutorial_Messages
    {
        public GameObject[] targetObjects;
    }


    public class Tutorial_Message_Controller_CS : MonoBehaviour
    {


        [SerializeField] Tutorial_Messages[] tutorialMessagesGroups = default;


        void Start()
        {
            for (int i = 0; i < tutorialMessagesGroups.Length; i++)
            {
                if (i == General_Settings_CS.Input_Type)
                {
                    continue;
                }

                for (int j = 0; j < tutorialMessagesGroups[i].targetObjects.Length; j++)
                {
                    var targetObject = tutorialMessagesGroups[i].targetObjects[j];
                    if (targetObject)
                    {
                        Destroy(targetObject);
                    }
                }
            }

            Destroy(gameObject);
        }


    }

}
