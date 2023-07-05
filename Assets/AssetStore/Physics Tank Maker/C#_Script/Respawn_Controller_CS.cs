using System.Collections;
using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Respawn_Controller_CS : MonoBehaviour
    {
        /*
		 * This script is attached to the top object of the tank.
		 * This script is used for respawning the tank in the runtime.
		 * The player can respawn the tank manually, also the tank is automatically respawned when the tank is destroyed.
		*/

        // User options >>
        public GameObject This_Prefab;
        public int Respawn_Times = 0;
        public float Auto_Respawn_Interval = 10.0f;
        public bool Remove_After_Death = false;
        public float Remove_Interval = 30.0f;
        public Transform Respawn_Point_Pack;
        public Transform Respawn_Target;
        // << User options


        Transform thisTransform;
        bool isAutoRespawning;
        Vector3 respawnPos;
        Quaternion respawnRot;
        Transform[] respawnPoints;
        bool isAnyTankRemoved;

        bool isSelected = true;


        void Start()
        {
            thisTransform = transform;

            // Set the respawn points.
            if (Respawn_Point_Pack)
            {
                respawnPoints = new Transform[Respawn_Point_Pack.childCount];
                for (int i = 0; i < Respawn_Point_Pack.childCount; i++)
                {
                    respawnPoints[i] = Respawn_Point_Pack.GetChild(i);
                }
            }
        }


        void Update()
        {
            if (isSelected == false)
            {
                return;
            }

            if (General_Settings_CS.Allow_Manual_Respawn)
            {
                Manual_Respawn();
            }
        }


        void Manual_Respawn()
        {
            // Respawn the tank Manually.
            if (Input.GetKeyDown(General_Settings_CS.Respawn_Key))
            {
                if (Respawn_Times > 0 && isAutoRespawning == false)
                {
                    Respawn();
                }
            }
        }


        void Respawn()
        {
            // Check the tank around the spawning point.
            StartCoroutine("Check_Spawning_Point");
        }


        IEnumerator Check_Spawning_Point()
        {
            while (Detect_Tank() == true)
            { // There is any tank in the spawning point.
                // Wait a second.
                yield return new WaitForSeconds(1.0f);
                yield return null;
            }
            // There is no tank in the spawning point.

            if (isAnyTankRemoved)
            { // Any destroyed tank has been removed.
                // Wait for the destroyed tank to be completely removed.
                yield return null;
            }

            // Spawn the tank.
            Spawn_Tank();
        }


        bool Detect_Tank()
        {
            // Set the respawn point.
            if (Respawn_Point_Pack == null || Respawn_Target == null)
            {
                respawnPos = transform.position;
                respawnRot = transform.rotation;
            }
            else
            {
                Get_Farthest_Point();
            }

            // Check the tank around the spawning point.
            Collider[] colliders = Physics.OverlapSphere(respawnPos, 5.0f, Layer_Settings_CS.Detect_Body_Layer_Mask);
            for (int i = 0; i < colliders.Length; i++)
            {
                var colliderRoot = colliders[i].transform.root;
                if (colliderRoot.tag != "Finish" && colliderRoot != thisTransform.root)
                { // There is a living tank in the area, && the tank is not this tank.
                    return true;
                }
            }
            // There is no living tank in the area.

            // Remove destroyed tanks in the area. 
            for (int i = 0; i < colliders.Length; i++)
            {
                Transform colliderRoot = colliders[i].transform.root;
                if (colliderRoot.tag == "Finish" && colliderRoot != transform.root)
                { // The tank is already dead, and it's not my self.
                    var respawnScript = colliders[i].GetComponentInParent<Respawn_Controller_CS>();
                    if (respawnScript)
                    {
                        if (respawnScript.Respawn_Times > 0)
                        { // The "ReSpawn_Times" of the tank remains.
                            return true;
                        }
                        else
                        { // The "ReSpawn_Times" of the tank does not remain.
                            // Remove the tank.
                            respawnScript.StartCoroutine("Remove_Tank", 0.0f);
                            isAnyTankRemoved = true;
                        }
                    }
                }
            }
            return false;
        }


        void Get_Farthest_Point()
        {
            if (Respawn_Target.root.tag == "Finish")
            { // The target tank already had been destroyed.
                respawnPos = transform.position;
                respawnRot = transform.rotation;
                return;
            }

            Rigidbody targetRigidbody = Respawn_Target.GetComponentInChildren<Rigidbody>();
            if (targetRigidbody == null)
            { // The target tank does not exist in the scene.
                respawnPos = transform.position;
                respawnRot = transform.rotation;
                return;
            }

            // Get the farthest posint.
            Transform targetTransform = targetRigidbody.transform;
            float maxDist = 0.0f;
            int farthestIndex = 0;
            for (int i = 0; i < respawnPoints.Length; i++)
            {
                float tempDist = Vector3.Distance(respawnPoints[i].position, targetTransform.position);
                if (tempDist > maxDist)
                {
                    maxDist = tempDist;
                    farthestIndex = i;
                }
            }
            respawnPos = respawnPoints[farthestIndex].position;
            respawnRot = respawnPoints[farthestIndex].rotation;
        }


        void Spawn_Tank()
        {
            // Make sure that "This_Prefab" is assigned.
            if (This_Prefab == null)
            {
                Debug.LogError("'The prefab for respawning is not assigned.");
                return;
            }

            // Reduce "ReSpawn_Times".
            Respawn_Times -= 1;

            // Destroy all the children.
            //(Note.) This GameObject is continuously used after the tank is respawned.
            int childCount = this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }

            // Instantiate the prefab.
            GameObject newTankObject = Instantiate(This_Prefab, respawnPos, respawnRot) as GameObject;

            // Make all the new child objects this children.
            childCount = newTankObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                newTankObject.transform.GetChild(0).parent = this.transform;
            }

            // Destroy the new parent object that has no child any longer.
            DestroyImmediate(newTankObject);

            // Set the tag.
            transform.root.tag = "Untagged";

            // Send "Respawned" message to components in this GameObject ("ID_Settings_CS", "AI_Headquaters_Helper_CS", "Special_Settings_CS", "UI_Position_Marker_Control_CS").
            this.gameObject.SendMessage("Respawned", SendMessageOptions.DontRequireReceiver);
        }


        IEnumerator Auto_Respawn()
        {
            isAutoRespawning = true; // To cancel the manual respawning.

            // Wait.
            yield return new WaitForSeconds(Auto_Respawn_Interval);
            isAutoRespawning = false;

            // Start respawning.
            Respawn();
        }


        public IEnumerator Remove_Tank(float interval)
        { // This function is called when the tank has been completely destroyed, also from "Event_Event_04_Remove_Tank_CS" script.
            if (isSelected)
            { // This tank is selected now.
                yield break;
            }

            // Wait.
            yield return new WaitForSeconds(interval);

            // Send "Prepare_Removing" message to components in this GameObject ("ID_Settings_CS", "AI_Headquaters_Helper_CS", "UI_Positon_Marker_Control_CS").
            this.gameObject.SendMessage("Prepare_Removing", SendMessageOptions.DontRequireReceiver);

            // Destroy this tank from the root.
            Destroy(transform.root.gameObject);
        }


        void MainBody_Destroyed_Linkage()
        { // Called from "Damage_Control_Center_CS".

            // Automatically respawn, or remove the tank.
            if (Respawn_Times > 0)
            { // "ReSpawn_Times" remains.
                // Start auto respawning.
                StartCoroutine("Auto_Respawn");
            }
            else
            { // "ReSpawn_Times" does not remain.
                if (Remove_After_Death)
                {
                    // Start removing the tank.
                    StartCoroutine("Remove_Tank", Remove_Interval);
                }
            }
        }


        void Selected(bool isSelected)
        { // Called from "ID_Settings_CS".
            this.isSelected = isSelected;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller".
            this.enabled = !isPaused;
        }

    }

}