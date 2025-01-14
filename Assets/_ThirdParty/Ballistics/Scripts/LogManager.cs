using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Ballistics.Scripts
{
    public class LogManager : MonoBehaviour {

        private static LogManager _instance;

        public static LogManager Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<LogManager>();
                    if (_instance == null) {
                        Debug.LogError("LogManager instance not found in scene!");
                    }
                }
                return _instance;
            }
        }

        public Text logText; // Public reference to the Text UI element in the inspector
        public ScrollRect scrollRect;

        void Awake() {
            // Ensure there's only one instance of LogManager
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            //DontDestroyOnLoad(gameObject); // Persists across scenes if needed

            // Clear any existing log messages
            logText.text = "";
        }

        public void ClearLog()
        {
            Debug.ClearDeveloperConsole();
            logText.text = "";
        }

        public void LogMessage(string message) {
            logText.text += message + "\n"; // Add message with newline
            Debug.Log(message);
        }
    }

}