
using Unity.Netcode;

namespace Factions
{
    public class ClientFactionsManager : FactionsManager
    {
        public override void Initialize()
        {
            OnClientInitializedServerRpc();
        }

        // Retreive assets from server
        [ServerRpc]
        private void OnClientInitializedServerRpc()
        {
            RetreiveAssetsFromServer();
        }

        private void RetreiveAssetsFromServer()
        {
            throw new System.NotImplementedException();
        }
    }
}