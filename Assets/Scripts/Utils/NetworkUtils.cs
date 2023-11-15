using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;

public class NetworkUtils {

    #region Scene management
    /// <summary>
    /// Load scene only for one connection.
    /// </summary>
    /// <param name="actualScene">Currently opened scene</param>
    /// <param name="sceneToLoad">Scene to load</param>
    /// <param name="networkConnection">Network connection for which the scene changes</param>
    public static void LoadScene(string actualScene, string sceneToLoad, NetworkConnection networkConnection) {
        var finder = InstanceFinder.NetworkManager.SceneManager;
        finder.LoadConnectionScenes(networkConnection, new SceneLoadData(sceneToLoad));
        finder.UnloadConnectionScenes(networkConnection, new SceneUnloadData(actualScene));
    }

    public static void LoadGlobalScene(string actualScene, string sceneToLoad) {
        var finder = InstanceFinder.NetworkManager.SceneManager;
        finder.LoadGlobalScenes(new SceneLoadData(sceneToLoad));
        finder.UnloadGlobalScenes(new SceneUnloadData(actualScene));
    }
    #endregion

}
