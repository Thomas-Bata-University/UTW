using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.UI;

public class PresetDropdown : MonoBehaviour
{
    public List<Preset> Presets;
    public Preset SelectedPreset;
    private Dropdown _presetDropdown;

    public Database assetDb;

    private void Start()
    {
        assetDb = GameObject.Find("AssetDatabase").GetComponent<Database>();

        _presetDropdown = GameObject.Find("PresetDropdown").GetComponent<Dropdown>();
        _presetDropdown.options.Clear();

        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.xml");

        var presets = files.Select(Deserialize).ToList();
        Presets = presets;
        foreach (var preset in Presets)
        {
            _presetDropdown.options.Add(new Dropdown.OptionData() { text = preset.presetName });
        }

        if (Presets != null) SelectedPreset = Presets[0];

        assetDb.SelectedPreset = SelectedPreset;
    }
    public void OnPresetSelected()
    {
        SelectedPreset = Presets.Find(x => x.presetName == _presetDropdown.options[_presetDropdown.value].text);

        assetDb.SelectedPreset = SelectedPreset;
    }

    private static Preset Deserialize(string path)
    {
        var serializer = new XmlSerializer(typeof(Preset));
        var reader = new StreamReader(path);
        var deserialized = (Preset)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }
}