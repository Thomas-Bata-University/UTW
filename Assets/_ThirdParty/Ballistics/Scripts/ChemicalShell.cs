using System;
using System.Linq;
using FishNet.Object;
using UnityEngine;
using Random = System.Random;

namespace Ballistics.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class ChemicalShell : NetworkBehaviour
    {
        [SerializeField] private float Caliber = 0.45f;
        [SerializeField] private int MinAngle = 70;
        [SerializeField] private int MaxAngle = 85;
        [SerializeField] private float JetLengthModifier = 1;
        [SerializeField] private Material stoppedMaterial;
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            //LogManager.Instance.ClearLog();
        }

        private void FixedUpdate()
        {
            // Cast a ray from out position, forward (relative to projectile), max distance calculated from current speed.
            var hitTargets = Physics.RaycastAll(transform.position, transform.forward,
                Time.fixedDeltaTime * Rigidbody.velocity.magnitude);

            if (!hitTargets.Any()) return;

            foreach (var hit in hitTargets)
            {
                var target = hit.collider.gameObject;

                // On terrain hit
                if (target.transform.TryGetComponent(out TerrainCollider _))
                {
                    //LogManager.Instance.LogMessage("Projectile hits the ground!");
                    
                    TurnOffRigidbody();
                    gameObject.transform.position = hit.point;
                    return;
                }

                // On Armor plate hit
                if (target.transform.TryGetComponent(out ArmorPlate armorPlate))
                {
                    // Load data
                    var impactNormal = transform.forward;
                    var armorNormal = hit.collider.transform.forward;
                    var impactAngle = Vector3.Angle(impactNormal, armorNormal);
                    if (impactAngle > 90) impactAngle = 180 - impactAngle;
                    
                    var armorThickness = armorPlate.GetArmorThickness();
                    var overmatch = Caliber / armorThickness;
                    
                    // Handle ricochet and overmatch
                    if (CalculateRicochetChance(impactAngle, MinAngle, MaxAngle, overmatch)) return;
                    //if (impactAngle > 70 && Caliber < armorThickness) return;
                    //LogManager.Instance.LogMessage("Projectile attempts to penetrate the armor!");
                    
                    var munroeLength = 0.1f * JetLengthModifier * Caliber;
                    var backcastOrigin = hit.point + Rigidbody.velocity.normalized * munroeLength;
                    if (Physics.Raycast(backcastOrigin, -Rigidbody.velocity.normalized, out var backRaycastHit, munroeLength))
                    {
                        Debug.DrawLine(backcastOrigin, backRaycastHit.point, Color.red,1200);
                        if(backRaycastHit.transform.TryGetComponent<ArmorPlate>(out var originalPlate))
                        {
                            originalPlate.Shatter(backRaycastHit.point, Rigidbody.velocity.normalized, 10 * Rigidbody.mass);
                            //LogManager.Instance.LogMessage("Shaped charge punched through the armor!");
                        }
                    } //else LogManager.Instance.LogMessage("Shaped charge failed to penetrate!");
                    
                    TurnOffRigidbody();
                    gameObject.transform.position = hit.point;
                    return;
                }

                if (hit.collider.transform.TryGetComponent(out TankModule tankModule))
                {
                    tankModule.TakeDamage(200);
                    Physics.IgnoreCollision(GetComponent<Collider>(), hit.collider);
                }
            }
        }
        private static bool CalculateRicochetChance(float impactAngle, float minAngle, float maxAngle, float overmatch)
        {
            var overmatchModifier = Mathf.Clamp01(Mathf.InverseLerp(1.3f, 7f, overmatch));
            var minRicochetAngle = minAngle + (overmatchModifier * (90f - minAngle));
            var maxRicochetAngle = maxAngle + (overmatchModifier * (90f - maxAngle));
            
            var clampedImpactAngle = Mathf.Clamp(impactAngle, minRicochetAngle, maxRicochetAngle);
            var normalizedAngle = (clampedImpactAngle - minRicochetAngle) / (maxRicochetAngle - minRicochetAngle);
            
            var random = new Random();
            var randomValue = random.NextDouble();
            //LogManager.Instance.LogMessage($"Ricochet calculation: Impact angle={impactAngle}, Min angle={minRicochetAngle}," +
             //                              $"Max angle={maxRicochetAngle}, Chance ={normalizedAngle}, Generated={randomValue}");
            return randomValue <= normalizedAngle;
        }
        private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;
        
        private void TurnOffRigidbody()
        {
            Rigidbody.isKinematic = true;
            Rigidbody.velocity = new Vector3(0, 0, 0);
            Rigidbody.useGravity = false;
            GetComponent<TrailRenderer>().material = stoppedMaterial;
        }
    }
}