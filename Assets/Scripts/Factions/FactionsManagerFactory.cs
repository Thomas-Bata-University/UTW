
using FishNet.Object;

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