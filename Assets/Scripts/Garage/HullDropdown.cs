using UnityEngine;
using UnityEngine.UI;

public class HullDropdown : MonoBehaviour
{
    private GameObject _assetDb;
    private Database _dbComponent;
    private Dropdown _dropdown;

    public void Start()
    {
        _dropdown = transform.GetComponent<Dropdown>();
        _dropdown.options.Clear();

        _assetDb = GameObject.Find("AssetDatabase");
        _dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

        Debug.Log(_dbComponent.hulls.Count == 0 ? "No hulls!" : "There are hulls!");

        foreach (var hull in _dbComponent.hulls)
        {
            _dropdown.options.Add(new Dropdown.OptionData
            {
                text = hull.name
            });
        }
    }
}