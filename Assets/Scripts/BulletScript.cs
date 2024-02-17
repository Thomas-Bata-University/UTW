using ChobiAssets.PTM;
using FishNet.Object;
using UnityEngine;

public class BulletScript : NetworkBehaviour
{
    public float AttackPoint = 50;
    void OnCollisionEnter(Collision collision)
    { // The collision has been detected by the physics engine.
        if(IsServer) AP_Hit_Process(collision.collider.gameObject, collision.relativeVelocity.magnitude, collision.contacts[0].normal);
    }

    void AP_Hit_Process(GameObject hitObject, float hitVelocity, Vector3 hitNormal)
    {
        if (hitObject == null)
        { // The hit object had been removed from the scene.
            return;
        }

        // Get the "Damage_Control_##_##_CS" script in the hit object.
        var damageScript = hitObject.GetComponent<Damage_Control_00_Base_CS>();
        if (damageScript != null)
        { // The hit object has "Damage_Control_##_##_CS" script. >> It should be a breakable object.

            // Calculate the hit damage.
            var hitAngle = Mathf.Abs(90.0f - Vector3.Angle(transform.forward, hitNormal));
            var damageValue = AttackPoint * Mathf.Pow(hitVelocity / 5.0f, 2.0f) * Mathf.Lerp(0.0f, 1.0f, Mathf.Sqrt(hitAngle / 90.0f));

            // Send the damage value to "Damage_Control_##_##_CS" script.
            if (damageScript.Get_Damage(damageValue, 0) == true)
            { // The hit part has been destroyed.
              // Remove the bullet from the scene.
                Destroy(this.gameObject);
            }
        }
    }
}
