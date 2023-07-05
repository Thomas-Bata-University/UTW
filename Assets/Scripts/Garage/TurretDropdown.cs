using UnityEngine;
using UnityEngine.UI;

public class TurretDropdown : MonoBehaviour
{
    private GameObject assetDb;
    private Database dbComponent;
    private Dropdown dropdown;
    public void GetAssetDb()
    {
        {
            dropdown = transform.GetComponent<Dropdown>();
            dropdown.options.Clear();

            assetDb = GameObject.Find("AssetDatabase");
            dbComponent = (Database)assetDb.GetComponent(typeof(Database));

            if (dbComponent.turrets.Count == 0) Debug.Log("No turrets!");
            else Debug.Log("There are turrets!");
            foreach (var turret in dbComponent.turrets)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = turret.name });
            }
        }
    }
}
