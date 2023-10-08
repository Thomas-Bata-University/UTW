using Unity.Netcode;

namespace Factions
{
    public class FactionsManagerFactory : NetworkBehaviour
    {
        public FactionsManager Create()
            => IsServer
                ? gameObject.AddComponent<ServerFactionsManager>()
                : gameObject.AddComponent<ClientFactionsManager>();
    }
}