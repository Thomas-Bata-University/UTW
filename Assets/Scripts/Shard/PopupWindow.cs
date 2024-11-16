using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button confirmButton;
    [SerializeField] private float fadeInDuration = 0.3f;

    private CanvasGroup _canvasGroup;
    private Action<string> _onConfirmWithSelection;

    private void Awake()
    {
        _canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = popupPanel.AddComponent<CanvasGroup>();

        // Set up confirm button listener
        confirmButton?.onClick.AddListener(() =>
        {
            var selectedValue = dropdown.options[dropdown.value].text;
            _onConfirmWithSelection?.Invoke(selectedValue);
            Hide();
        });

        // Hide popup by default
        popupPanel.SetActive(false);
    }

    public void Show(string title, string message, List<string> options, Action<string> onConfirmAction = null)
    {
        titleText.text = title;
        messageText.text = message;
        _onConfirmWithSelection = onConfirmAction;

        // Clear and populate dropdown
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // Default to first option
        dropdown.value = 0;

        // Show the popup
        popupPanel.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public string GetSelectedValue()
    {
        return dropdown.options[dropdown.value].text;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        _canvasGroup.alpha = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = elapsedTime / fadeInDuration;
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    private void Hide()
    {
        popupPanel.SetActive(false);
    }
}