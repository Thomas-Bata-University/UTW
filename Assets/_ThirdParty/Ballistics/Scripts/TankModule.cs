using FishNet.Object;
using UnityEngine;

namespace Ballistics.Scripts
{
    public class TankModule : NetworkBehaviour
    {
        [SerializeField] private TankModuleState state = TankModuleState.Operational;
        [SerializeField] private int maxHitPoints = 300;
        [SerializeField] private int hitPoints = 300;
        [SerializeField] private bool isSoft = false;
        [SerializeField] private Material moduleOkMaterial;
        [SerializeField] private Material moduleDamagedMaterial;
        [SerializeField] private Material moduleDestroyedMaterial;
        [SerializeField] private MeshRenderer meshRenderer;
        private VehicleManager _vehicleManager;

        public void Start()
        {
            
            _vehicleManager = GetRootGameObject().GetComponent<VehicleManager>();
            hitPoints = maxHitPoints;
        }

        public void TakeDamage(int damage)
        {
            if (state.Equals(TankModuleState.Destroyed)) return;
            if (damage - hitPoints > 2 * maxHitPoints || (hitPoints - damage < 0 && state.Equals(TankModuleState.Damaged)))
            {
                hitPoints = 0;
                state = TankModuleState.Destroyed;
                meshRenderer.material = moduleDestroyedMaterial;
                ChangeColorRpc();
                if (isSoft)
                { 
                    if(TryGetComponent<Collider>(out var collider))
                    {
                        collider.enabled = false;
                    }
                }
                _vehicleManager.ShellHitsVehicle();
                //LogManager.Instance.LogMessage($"{this.gameObject.name} has been struck and took {damage} points of damage and is now destroyed");
            } else if (hitPoints - damage < 0)
            {
                hitPoints = 0;
                state = TankModuleState.Damaged;
                ChangeColorRpc();
                meshRenderer.material = moduleDamagedMaterial;
                //LogManager.Instance.LogMessage($"{this.gameObject.name} has been struck and took {damage} points of damage and is now damaged");
            }
            else
            {
                hitPoints -= damage;
                //LogManager.Instance.LogMessage($"{this.gameObject.name} has been struck and took {damage} points of damage");
            }
        }

        [ObserversRpc]
        void ChangeColorRpc()
        {
            meshRenderer.material = state switch
            {
                TankModuleState.Operational => moduleOkMaterial,
                TankModuleState.Destroyed => moduleDamagedMaterial,
                TankModuleState.Damaged => moduleDestroyedMaterial,
                _ => meshRenderer.material
            };
        }

        GameObject GetRootGameObject()
        {
            var root = transform;
            while (root.parent != null) root = root.parent;
            return root.gameObject;
        }
    }
    
    

    public enum TankModuleState
    {
        Operational,
        Damaged,
        Destroyed
    }
}