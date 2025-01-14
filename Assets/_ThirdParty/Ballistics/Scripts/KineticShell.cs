using System;
using System.Linq;
using FishNet.Object;
using UnityEngine;
using Random = System.Random;

namespace Ballistics.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class KineticShell : NetworkBehaviour
    {
        [SerializeField] private float Caliber = 0.45f;
        [SerializeField] private int MinAngle = 45;
        [SerializeField] private int MaxAngle = 60;
        [SerializeField] private float ShapeFactor = 1.4f;
        [SerializeField] private Material stoppedMaterial;

        private bool isInitialized = false;
        
        private Collider lastHitCollider = null;
        public Rigidbody Rigidbody { get; private set; }

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            isInitialized = true;
            //LogManager.Instance.ClearLog();
        }

        private void FixedUpdate()
        {
            if(IsServer && isInitialized) HandleBallistics();
        }

        private void HandleBallistics()
        {
                        // Cast a ray from out position, forward (relative to projectile), max distance calculated from current speed.
            var hitTargets = Physics.RaycastAll(transform.position, transform.forward,
                Time.fixedDeltaTime * Rigidbody.velocity.magnitude);

            if (!hitTargets.Any()) return;

            foreach (var hit in hitTargets)
            {
                if (lastHitCollider == hit.collider) continue;
                var target = hit.collider.gameObject;

                // On terrain hit
                if (target.transform.TryGetComponent(out TerrainCollider _))
                {
                    Debug.Log("Projectile hits the ground!");
                    TurnOffRigidbody();
                    gameObject.transform.position = hit.point;
                    return;
                }

                // On Armor plate hit
                if (target.transform.TryGetComponent(out ArmorPlate armorPlate))
                {
                    // Load data
                    var velocity = Rigidbody.velocity.magnitude;
                    var impactAngle = Vector3.Angle(transform.forward, hit.collider.transform.forward);
                    if (impactAngle > 90) impactAngle = 180 - impactAngle;
                    var impactMass = Rigidbody.mass;
                    var armorQuality = armorPlate.GetArmorQuality();
                    var armorThickness = armorPlate.GetArmorThickness();
                    var overmatch = Caliber / armorThickness;
                    
                    // Handle ricochet and overmatch
                    if (CalculateRicochetChance(impactAngle, MinAngle, MaxAngle, overmatch)) return;
                    Debug.Log("Projectile attempts to penetrate the armor!");
                    
                    // DeMarre penetration approximation
                    var dmPenetrated = Math.Abs(
                        Math.Pow(velocity * Math.Pow(impactMass, 0.5) * Math.Pow(Math.Cos(DegToRad(impactAngle)), ShapeFactor)
                                 / (armorQuality * Math.Pow(Caliber, 0.75)), 1D));

                    Debug.Log($"A shell with a velocity of {velocity}m/s, with angle of {impactAngle}°," +
                                                  $"mass of {impactMass}kg, caliber of {Caliber}dm and penetration power {dmPenetrated}dm, just hit an armor plate!");
                    // Trigger effects or actions based on penetration depth
                    if (dmPenetrated < armorThickness && dmPenetrated > armorThickness / 1.1)
                    {
                        Debug.Log("Partial penetration!");
                        
                        armorPlate.Shatter(hit.point + Rigidbody.velocity.normalized * 0.01f, Rigidbody.velocity.normalized, (Caliber+armorThickness)*10);
                        
                        TurnOffRigidbody();
                        gameObject.transform.position = hit.point;
                        return;
                    }
                    else if (dmPenetrated > armorThickness)
                    {
                        Debug.Log("Full penetration!");
                        
                        Ignore(hit.collider);
                        
                        //Offset the hit.point a bit to ignore armor plate collider
                        armorPlate.Shatter(hit.point + Rigidbody.velocity.normalized * 0.01f, Rigidbody.velocity.normalized, (Caliber+armorThickness)*20);

                        gameObject.transform.position = hit.point;
                        Slowdown((float)(armorThickness / dmPenetrated));
                    }
                    else
                    {
                        Debug.Log("Failed to penetrate!");

                        TurnOffRigidbody();
                        gameObject.transform.position = hit.point;
                        return;
                    }
                }

                if (hit.collider.transform.TryGetComponent(out TankModule tankModule))
                {
                    tankModule.TakeDamage(200);
                    Ignore(hit.collider);
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
            Debug.Log($"Ricochet calculation: Impact angle={impactAngle}, Min angle={minRicochetAngle}," +
                                           $"Max angle={maxRicochetAngle}, Chance ={normalizedAngle}, Generated={randomValue}");
            return randomValue <= normalizedAngle;
        }
        private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;

        private void Slowdown(float amount)
        {
            Rigidbody.velocity -= Rigidbody.velocity * amount;
            Debug.Log($"New velocity ={Rigidbody.velocity.magnitude}");
        }

        private void Ignore(Collider collider)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collider);
            lastHitCollider = collider;
        }

        private void TurnOffRigidbody()
        {
            Rigidbody.isKinematic = true;
            Rigidbody.velocity = new Vector3(0, 0, 0);
            Rigidbody.useGravity = false;
            GetComponent<TrailRenderer>().material = stoppedMaterial;
        }
    }
}