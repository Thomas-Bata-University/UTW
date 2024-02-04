using FishNet;
using FishNet.Connection;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PresetDropdown : MonoBehaviour {

    public static UnityAction<NetworkConnection, Preset> OnPresetChange;

    [Header("UI")]
    public TMP_Dropdown _presetDropdown;

    private List<Preset> presets;
    private Preset selectedPreset;
    private Database assetDatabase;

    private void Start() {
        if (InstanceFinder.IsServer) return;
        assetDatabase = FindObjectOfType<Database>();

        _presetDropdown.options.Clear();

        presets = assetDatabase.presetList;
        foreach (var preset in presets) {
            _presetDropdown.options.Add(new TMP_Dropdown.OptionData() { text = preset.presetName });
        }

        if (presets.Count == 0) {
            Debug.Log("No preset found.");
            return;
        }

        selectedPreset = presets[0]; //Default
        _presetDropdown.captionText.text = presets[0].presetName;
        assetDatabase.SelectedPreset = selectedPreset;
    }
    public void OnPresetSelected() {
        selectedPreset = presets.Find(x => x.presetName == _presetDropdown.options[_presetDropdown.value].text);
        _presetDropdown.captionText.text = selectedPreset.presetName;

        assetDatabase.SelectedPreset = selectedPreset;
        OnPresetChange?.Invoke(InstanceFinder.ClientManager.Connection, selectedPreset);
    }

}