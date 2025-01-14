using System;
using FishNet.Object;
using UnityEngine;
using Random = System.Random;

namespace Ballistics.Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class ArmorPlate : NetworkBehaviour
    {
        [SerializeField] private int ArmorQuality = 2000 ;
        [SerializeField] private float TinyFragmentChance = 0.7f;
        [SerializeField] private float MediumFragmentChance = 0.2f;
        [SerializeField] private float SizableFragmentChance = 0.1f;
        [SerializeField] private BoxCollider boxCollider;

        private const int TinyCost = 1; 
        private const int MediumCost = 3; 
        private const int SizableCost = 5; 
        
        private float ArmorThickness = 0;

        private void Start()
        {
            ArmorThickness = boxCollider.size.z * gameObject.transform.localScale.z * 10;
        }

        public int GetArmorQuality() => ArmorQuality;
        public float GetArmorThickness() => ArmorThickness;

        public void Shatter(Vector3 origin, Vector3 direction, float points)
        {
            var pointsLeft = points;
            while (pointsLeft > 0)
            {
                var randomValue = UnityEngine.Random.value;
                if (randomValue < SizableFragmentChance && pointsLeft >= SizableCost)
                {
                    GenerateSpalling(origin, AddRandomOffset(direction, 1300),300, Color.black );
                    pointsLeft -= SizableCost;
                }
                if (randomValue < MediumFragmentChance && pointsLeft >= MediumCost)
                {
                    GenerateSpalling(origin, AddRandomOffset(direction, 1700),120, Color.blue );
                    pointsLeft -= MediumCost;
                }
                else
                {
                    GenerateSpalling(origin, AddRandomOffset(direction, 2100),50, Color.cyan );
                    pointsLeft -= TinyCost;
                }
            }
        }

        public void GenerateSpalling(Vector3 origin,Vector3 direction, int damage, Color color)
        {
            var hit = Physics.Raycast(origin, direction, out var target, 10);
            if (!hit) return;
            if(target.transform.TryGetComponent<TankModule>(out var module)) module.TakeDamage(damage); 
            Debug.DrawLine(origin, target.point, color,1200);
            //LogManager.Instance.LogMessage($"Spall has struck the {target.transform.name}");
        }
        
        public Vector3 AddRandomOffset(Vector3 direction,float maxOffsetAngle) {
            var maxOffsetRadians = Mathf.Deg2Rad * maxOffsetAngle;
            
            var randomXOffset = UnityEngine.Random.Range(-maxOffsetRadians, maxOffsetRadians);
            var randomYOffset = UnityEngine.Random.Range(-maxOffsetRadians, maxOffsetRadians);
            var offset = new Vector3(randomXOffset, randomYOffset, 0f);
            
            var rotatedDirection = Quaternion.Euler(offset) * direction;
            return rotatedDirection.normalized;
        }
    }
}