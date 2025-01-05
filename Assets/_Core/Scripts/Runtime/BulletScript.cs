using ChobiAssets.PTM;
using FishNet.Object;
using UnityEngine;

public class BulletScript : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    { // The collision has been detected by the physics engine.
        if(IsServer) AP_Hit_Process(collision.collider.gameObject);
    }

    void AP_Hit_Process(GameObject hitObject)
    {
        if (hitObject == null) return;

        var targetVehicleManager = hitObject.GetComponentInParent<VehicleManager>();
        if (targetVehicleManager != null) targetVehicleManager.ShellHitsVehicle();
    }
}
