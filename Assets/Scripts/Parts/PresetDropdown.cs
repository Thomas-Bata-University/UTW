using FishNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PresetDropdown : MonoBehaviour {

    [Header("UI")]
    public Dropdown _presetDropdown;

    private List<Preset> Presets;
    private Preset SelectedPreset;
    private Database assetDatabase;
    public delegate void PresetSelect(Preset preset);
    public static event PresetSelect OnPresetChanged;
    private void Start() {
        if (InstanceFinder.IsServer) return;
        assetDatabase = FindObjectOfType<Database>();

        _presetDropdown.options.Clear();

        Presets = assetDatabase.presetList;
        foreach (var preset in Presets) {
            _presetDropdown.options.Add(new Dropdown.OptionData() { text = preset.presetName });
        }

        if (Presets.Count == 0) {
            Debug.Log("No preset found.");
            return;
        }

        SelectedPreset = Presets[0]; //Default
        assetDatabase.SelectedPreset = SelectedPreset;
    }
    public void OnPresetSelected() {
        SelectedPreset = Presets.Find(x => x.presetName == _presetDropdown.options[_presetDropdown.value].text);

        assetDatabase.SelectedPreset = SelectedPreset;
        OnPresetChanged.Invoke(SelectedPreset);

    }

}