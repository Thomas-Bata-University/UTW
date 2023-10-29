using UnityEngine;
using System.IO;
using ChobiAssets.PTM;
using FishNet.Component.Transforming;
using Parts;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GaragePreview : MonoBehaviour
{
    public string _MainMenuScene;
    private GameObject assetDb;
    public Database dbComponent;
    private Dropdown hullDropdown;
    private Dropdown turretDropdown;

    GameObject selectedHull;
    GameObject selectedTurret;

    GameObject instantiatedHull;
    GameObject instantiatedTurret;
    public void Start()
    {
        {
            assetDb = GameObject.Find("AssetDatabase");
            dbComponent = (Database)assetDb.GetComponent(typeof(Database));

            Preview();
        }
    }

    public void Preview()
    {
        hullDropdown = GameObject.Find("HullDropdown").GetComponent<Dropdown>();
        turretDropdown = GameObject.Find("TurretDropdown").GetComponent<Dropdown>();

        if (instantiatedHull != null || instantiatedTurret != null) CleanInstances();

        selectedHull = dbComponent.hulls.Find(x => x.name == hullDropdown.options[hullDropdown.value].text);
        if(selectedHull == null) Debug.Log("No hull selected!");

        selectedTurret = dbComponent.turrets.Find(x => x.name == turretDropdown.options[turretDropdown.value].text);
        if (selectedTurret == null) Debug.Log("No turret selected!");

        NetworkTransform mainbodyNt = selectedHull.GetComponentInChildren<NetworkTransform>();
        DestroyImmediate(mainbodyNt, true);

        NetworkTransform[] turretNts = selectedTurret.GetComponentsInChildren<NetworkTransform>();
        foreach (var nt in turretNts)
        {
            DestroyImmediate(nt, true);
        }

        instantiatedHull = Instantiate(selectedHull, new Vector3(0, 0, 0), Quaternion.Euler(0, 120, 0));
        
        Transform turretGO = instantiatedHull.transform.Find("TurretMount");
        Vector3 turretMount = turretGO.position;
        instantiatedTurret = Instantiate(selectedTurret, turretMount, Quaternion.Euler(0, 120, 0));
        instantiatedTurret.transform.SetParent(instantiatedHull.transform);
        instantiatedTurret.transform.GetComponentInChildren<Cannon_Fire_CS>().enabled = false;
        instantiatedTurret.transform.GetComponentInChildren<Cannon_Vertical_CS>().enabled = false;
        instantiatedTurret.transform.GetComponentInChildren<Turret_Horizontal_CS>().enabled = false;
        
    }

    public void SavePreset()
    {
        string input = GameObject.Find("PresetInputField").GetComponent<InputField>().text;
        Debug.Log("Text in inputfield: " + input);
        if (input != null){

            Preset tonk = new Preset();
            tonk.presetName = input;

            tonk.hull = hullDropdown.options[hullDropdown.value].text;
            if (tonk.hull == null) Debug.Log("No hull selected!");

            tonk.turret = turretDropdown.options[turretDropdown.value].text;
            if (selectedTurret == null) Debug.Log("No turret selected!");

            string json = JsonUtility.ToJson(tonk);
            if (!Directory.Exists(Application.streamingAssetsPath + "/Presets/")) Directory.CreateDirectory(Application.streamingAssetsPath + "/Presets/");
            string filePath = Path.Combine(Application.streamingAssetsPath, "Presets", tonk.presetName + ".json");
            File.WriteAllText(filePath, json);
        }
        SceneManager.LoadScene(_MainMenuScene);
    }

    public void MainMenuDialog()
    {
        SceneManager.LoadScene(_MainMenuScene);
    }

    private void CleanInstances()
    {
        if(instantiatedHull != null) Destroy(instantiatedHull);
        Debug.Log("Hull Destroyed!");

        if (instantiatedTurret != null) Destroy(instantiatedTurret);
        Debug.Log("Turret Destroyed!");
    }
}
