using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.UI;

public class PresetDropdown : MonoBehaviour
{
    public List<Preset> presets;
    public Preset selectedPreset;
    Dropdown presetDropdown;

    public Database assetDb;

    void Start()
    {
        assetDb = GameObject.Find("AssetDatabase").GetComponent<Database>();

        List<Preset> p = new List<Preset>();
        presetDropdown = GameObject.Find("PresetDropdown").GetComponent<Dropdown>();
        presetDropdown.options.Clear();

        string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/Presets/", "*.xml");

        foreach (string file in files)
        {
            Preset deserialized = Deserialize(file);
            if (p == null) p[0] = deserialized;
            else p.Add(deserialized);
        }
        presets = p;
        foreach (Preset preset in presets)
        {
            presetDropdown.options.Add(new Dropdown.OptionData() { text = preset.presetName });
        }

        if (presets != null) selectedPreset = presets[0];

        assetDb.SelectedPreset = selectedPreset;
    }
    public void onPresetSelected()
    {
        selectedPreset = presets.Find(x => x.presetName == presetDropdown.options[presetDropdown.value].text);

        assetDb.SelectedPreset = selectedPreset;
    }

    Preset Deserialize(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Preset));
        StreamReader reader = new StreamReader(path);
        Preset deserialized = (Preset)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }
}