using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

    public class Recoil_Brake_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the "Barrel_base" in the tank.
		 * This script controls the barrel motion in the tank.
		 * When firing, this script moves the barrel backward and forward like a recoil brake.
		*/

        // User options >>
        public float Total_Time = 1.0f;
        public float Recoil_Length = 0.4f;
        public AnimationCurve Motion_Curve;
        // << User options


        public int Barrel_Type = 0; // Set by "Barrel_Base".(0 = Single barrel, 1 = Left of twins, 2 = Right of twins)

        bool isReady = true;
        Transform thisTransform;
        Vector3 initialLocalPosition;


        void Start()
        {
            Initialize();
        }


        void Initialize()
        { // This function must be called in Start() after changing the hierarchy.
            thisTransform = transform;
            initialLocalPosition = thisTransform.localPosition;
            if (Recoil_Length != 0.0f && Motion_Curve.keys.Length < 3)
            { // The "Motion_Curve" is not set yet.
                Create_Curve();
            }
        }


        void Create_Curve()
        { // Create a temporary AnimationCurve.
            Debug.LogWarning("'Motion Curve' is not set correctly in 'Recoil_Brake_CS'.");
            Keyframe key1 = new Keyframe(0.0f, 0.0f, 11.0f, 11.0f);
            Keyframe key2 = new Keyframe(0.2f, 1.0f, 0.01895372f, 0.01895372f);
            Keyframe key3 = new Keyframe(1.0f, 0.0f, -0.02f, -0.02f);
            Motion_Curve = new AnimationCurve(key1, key2, key3);
        }


        public void Fire_Linkage(int direction)
        { // Called from "Cannon_Fire_CS".
            if (isReady == false)
            {
                return;
            }

            if (Barrel_Type == 0 || Barrel_Type == direction)
            { // Single barrel, or the same direction.
                StartCoroutine("Recoil_Brake");
            }
        }


        IEnumerator Recoil_Brake()
        {
            isReady = false;

            var count = 0.0f;
            Vector3 currentLocalPosition = initialLocalPosition;
            while (count < Total_Time)
            {
                var rate = Motion_Curve.Evaluate(count / Total_Time);
                currentLocalPosition.z = initialLocalPosition.z - (rate * Recoil_Length);
                thisTransform.localPosition = currentLocalPosition;
                count += Time.deltaTime;
                yield return null;
            }
            thisTransform.localPosition = initialLocalPosition;

            isReady = true;
        }


        void Turret_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".
            Destroy(this);
        }

    }

}