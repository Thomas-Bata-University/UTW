using FishNet.Transporting.Tugboat;
using UnityEngine;

public class Menu : MonoBehaviour {

    [SerializeField] private GameObject networkManager;
    [SerializeField] private GameObject presetManager;
    private Tugboat _tugboat;

    private void Start() {
        if (networkManager.TryGetComponent(out Tugboat t)) {
            _tugboat = t;
        } else {
            Debug.LogError("Couldn't find Tugboat component!");
        }
    }

    public void Host() {
        _tugboat.StartConnection(true);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Join() {
        _tugboat.StartConnection(false);
    }

}
