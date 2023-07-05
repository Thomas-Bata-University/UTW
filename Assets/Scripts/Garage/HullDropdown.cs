using UnityEngine;
using UnityEngine.UI;

public class HullDropdown : MonoBehaviour
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

            if (dbComponent.hulls.Count == 0) Debug.Log("No hulls!");
            else Debug.Log("There are hulls!");
            foreach (var hull in dbComponent.hulls)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = hull.name });
            }
        }
    }
}
