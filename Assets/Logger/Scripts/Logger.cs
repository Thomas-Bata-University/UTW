using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Runtime.CompilerServices;
using FishNet.Component.Utility;
using System.Net.NetworkInformation;

public enum LogPosition { Top, Bottom }

public class Logger : MonoBehaviour
{
    [SerializeField] LogPosition position = LogPosition.Top;
    [Range(200f, 800f)][SerializeField] private float height = 300f;
    [SerializeField] private Sprite spriteOpenIcon;
    [SerializeField] private Sprite spriteCloseIcon;
    [Space(50f)]
    [SerializeField] private GameObject uiEventSystem;
    [SerializeField] private Button uiToggleButton;
    [SerializeField] private TextMeshProUGUI uiLogText;
    [SerializeField] private GameObject uiViewport;
    [SerializeField] private GameObject uiScrollBar;
    [SerializeField] private ScrollRect uiScrollRect;

    private VerticalLayoutGroup uiContentVerticalLayoutGroup;
    private RectTransform uiScrollRectTransform;
    private Image uiToggleButtonImage;

    private KeyCode consoleKey = KeyCode.C;
    private KeyCode pingKey = KeyCode.R;
    private KeyCode fishUiKey = KeyCode.F;

    GameObject uiNetworkHudCanvas;

    private bool isOpen = false;
    private bool isPing = true;
    private bool isFish = true;

    private string[] colors = new string[3]{
            "#aaaaaa", // White
            "#ffdd33", // Yellow
            "#ff6666"  // Red
        };

    private void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
        uiToggleButton.onClick.AddListener(ToggleLogUI);
    }

    private void Awake()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
            uiEventSystem.SetActive(false);

        uiContentVerticalLayoutGroup = transform.GetChild(0).GetComponent<VerticalLayoutGroup>();
        uiScrollRectTransform = uiScrollRect.GetComponent<RectTransform>();
        uiToggleButtonImage = uiToggleButton.GetComponent<Image>();

        uiNetworkHudCanvas = GameObject.Find("NetworkHudCanvas");
        ToggleFishUi();
        TogglePingDisplay();

        isOpen = true;
        ToggleLogUI();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(consoleKey))
            {
                ToggleLogUI();
            }

            if (Input.GetKeyDown(pingKey))
            {
                TogglePingDisplay();
            }

            if (Input.GetKeyDown(fishUiKey))
            {
                ToggleFishUi();
            }
        }
    }

    private void ScrollDown()
    {
        uiScrollRect.verticalNormalizedPosition = 0f;
    }

    private void LogCallback(string message, string stackTrace, LogType type)
    {
        //logTypeIndex => normal:0 , warning:1 , error:2
        int logTypeIndex = (type == LogType.Log) ? 0 : (type == LogType.Warning) ? 1 : 2;

        if (type == LogType.Warning)
            return;

        uiLogText.text += $"<sprite={logTypeIndex}><color={colors[logTypeIndex]}> {message}</color>\n\n";
        ScrollDown();
    }

    private void ToggleFishUi()
    {
        isFish = !isFish;

        uiNetworkHudCanvas.SetActive(isFish);
    }

    private void TogglePingDisplay()
    {
        isPing = !isPing;
        gameObject.GetComponent<PingDisplay>().enabled = isPing;
    }

    private void ToggleLogUI()
    {
        isOpen = !isOpen;
        if (isOpen)
            SetupUI(new Vector2(1f, height), spriteCloseIcon);
        else
        {
            SetupUI(Vector2.one * 43f, spriteOpenIcon);
            ScrollDown();
        }
    }

    private void SetupUI(Vector2 size, Sprite icon)
    {
        uiScrollRect.enabled = isOpen;
        uiViewport.SetActive(isOpen);
        uiScrollBar.SetActive(isOpen);
        uiContentVerticalLayoutGroup.childForceExpandWidth = isOpen;
        uiContentVerticalLayoutGroup.childControlWidth = isOpen;
        uiScrollRectTransform.sizeDelta = size;
        uiToggleButtonImage.sprite = icon;
    }

    private void OnValidate()
    {
        if (uiContentVerticalLayoutGroup != null)
        {
            if (position == LogPosition.Top)
                uiContentVerticalLayoutGroup.childAlignment = TextAnchor.UpperRight;
            else
                uiContentVerticalLayoutGroup.childAlignment = TextAnchor.LowerRight;
        }
    }

    private void OnDisable()
    {
        Application.logMessageReceived += LogCallback;
        uiToggleButton.onClick.RemoveListener(ToggleLogUI);
    }
}