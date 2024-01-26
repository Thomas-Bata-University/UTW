using UnityEngine;
using UnityEngine.UI;

public class TurretDropdown : MonoBehaviour
{
    private GameObject assetDb;
    private Database dbComponent;
    private Dropdown dropdown;
    public void Start()
    {
        {
            dropdown = transform.GetComponent<Dropdown>();
            dropdown.options.Clear();

            assetDb = GameObject.Find("AssetDatabase");
            dbComponent = (Database)assetDb.GetComponent(typeof(Database));

            Debug.Log(dbComponent.turrets.Count == 0 ? "No turrets!" : "There are turrets!");
            foreach (var turret in dbComponent.turrets)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = turret.name });
            }
        }
    }
}
