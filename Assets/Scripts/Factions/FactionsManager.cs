using System.Threading.Tasks;
using Unity.Netcode;

namespace Factions
{
    public abstract class FactionsManager : NetworkBehaviour
    {
        public abstract void Initialize();
    }


}