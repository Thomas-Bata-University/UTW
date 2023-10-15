using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
<<<<<<< HEAD
using System.IO;
using System.Linq;
=======
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
using UnityEngine.UI;

public class PresetDropdown : MonoBehaviour {
    public List<Preset> Presets;
    public Preset SelectedPreset;
    private Dropdown _presetDropdown;

    public Database assetDb;
    private PresetManager presetManager;

    private void Start() {
        assetDb = FindObjectOfType<Database>();
        presetManager = FindObjectOfType<PresetManager>();
        presetManager.LoadPresetServerRpc(NetworkManager.Singleton.LocalClientId);

        _presetDropdown = GameObject.Find("PresetDropdown").GetComponent<Dropdown>();
        _presetDropdown.options.Clear();

        Presets = assetDb.presetList;
        foreach (var preset in Presets) {
            _presetDropdown.options.Add(new Dropdown.OptionData() { text = preset.PresetName });
        }

        if (Presets != null) SelectedPreset = Presets[0];

        assetDb.SelectedPreset = SelectedPreset;
    }
    public void OnPresetSelected() {
        SelectedPreset = Presets.Find(x => x.PresetName == _presetDropdown.options[_presetDropdown.value].text);

        assetDb.SelectedPreset = SelectedPreset;
    }

}