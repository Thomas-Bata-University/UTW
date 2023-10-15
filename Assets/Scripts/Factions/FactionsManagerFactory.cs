using Unity.Netcode;

namespace Factions
{
    public class FactionsManagerFactory : NetworkBehaviour
    {
        public IFactionManager Create()
            => IsServer
                ? new ServerFactionsManager()
                : new ClientFactionsManager();
    }
}