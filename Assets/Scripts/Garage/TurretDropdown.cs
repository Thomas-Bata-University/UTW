using UnityEngine;
using UnityEngine.UI;

public class TurretDropdown : MonoBehaviour
{
    private GameObject _assetDb;
    private Database _dbComponent;
    private Dropdown _dropdown;
    public void Start()
    {
        {
            _dropdown = transform.GetComponent<Dropdown>();
            _dropdown.options.Clear();

            _assetDb = GameObject.Find("AssetDatabase");
            _dbComponent = (Database)_assetDb.GetComponent(typeof(Database));

            Debug.Log(_dbComponent.turrets.Count == 0 ? "No turrets!" : "There are turrets!");
            foreach (var turret in _dbComponent.turrets)
            {
                _dropdown.options.Add(new Dropdown.OptionData
                {
                    text = turret.name
                });
            }
        }
    }
}
