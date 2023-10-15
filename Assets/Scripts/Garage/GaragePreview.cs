<<<<<<< HEAD
using UnityEngine;
using System.IO;
using ChobiAssets.PTM;
using FishNet.Component.Transforming;
using UnityEngine.UI;
=======
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
>>>>>>> 114f795 (Basic save and load asset from SERVER, comunication between instances)
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GaragePreview : MonoBehaviour {

    [Header("UI")]
    public Dropdown hullDropdown;
    public Dropdown turretDropdown;
    public InputField presetNameInputField;

    private Database assetDatabase;
    private PresetManager presetManager;

    private GameObject instantiatedHull;
    private GameObject instantiatedTurret;

    private void Start() {
        assetDatabase = FindObjectOfType<Database>();
        presetManager = FindObjectOfType<PresetManager>();

        Preview(assetDatabase.hulls[0], assetDatabase.turrets[0]);
    }

    public void Preview(GameObject hull, GameObject turret) {
        if (instantiatedHull != null || instantiatedTurret != null) CleanInstances();

        GameObject selectedHull = hull;
        if (selectedHull == null) Debug.Log("No hull selected!");

        GameObject selectedTurret = turret;
        if (selectedTurret == null) Debug.Log("No turret selected!");

        NetworkTransform mainbodyNt = selectedHull.GetComponentInChildren<NetworkTransform>();
        DestroyImmediate(mainbodyNt, true);

        NetworkTransform[] turretNts = selectedTurret.GetComponentsInChildren<NetworkTransform>();
        foreach (var nt in turretNts) {
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
    public void Preview() {
        GameObject selectedHull = assetDatabase.hulls.Find(x => x.name == hullDropdown.options[hullDropdown.value].text);
        GameObject selectedTurret = assetDatabase.turrets.Find(x => x.name == turretDropdown.options[turretDropdown.value].text);
        Preview(selectedHull, selectedTurret);
    }

    public void MainMenuDialog() {
        SceneManager.LoadScene(GameSceneUtils.MAIN_MENU_SCENE);
    }

    private void CleanInstances() {
        if (instantiatedHull != null) Destroy(instantiatedHull);
        Debug.Log("Hull Destroyed!");

        if (instantiatedTurret != null) Destroy(instantiatedTurret);
        Debug.Log("Turret Destroyed!");
    }

    public Preset CreatePreset(string presetName) {
        string hull = hullDropdown.options[hullDropdown.value].text;
        string turret = turretDropdown.options[turretDropdown.value].text;
        Preset tankPreset = new Preset(presetName, hull, turret);

        return tankPreset;
    }

    public void SavePreset() {
        string presetName = presetNameInputField.text;

        if (presetName == null) {
            Debug.LogWarning($"Cannot save preset because of input: {presetName}");
            return;
        }

        Debug.Log("Preset name: " + presetName);
        presetManager.SavePresetServerRpc(CreatePreset(presetName), NetworkManager.Singleton.LocalClientId);
    }

}
