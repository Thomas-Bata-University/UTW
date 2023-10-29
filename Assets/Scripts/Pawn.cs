using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace DefaultNamespace
{
    public sealed class Pawn : NetworkBehaviour
    {
        [SyncVar] public Player controllingPlayer;

        [SyncVar] public float health;

        public void ReceiveDamage(float amount)
        {
            if (!IsSpawned) return;

            if ((health -= amount) <= 0.0f)
            {
                controllingPlayer.TargetPawnKilled(Owner);

                Despawn();
            }
        }
    }
}