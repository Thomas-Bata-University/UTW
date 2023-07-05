using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

    [CustomEditor(typeof(Track_Scroll_CS))]
    public class Track_Scroll_CSEditor : Editor
    {

        SerializedProperty Is_LeftProp;
        SerializedProperty Reference_WheelProp;
        SerializedProperty Reference_NameProp;
        SerializedProperty Reference_Parent_NameProp;
        SerializedProperty Scroll_RateProp;
        SerializedProperty Scroll_Y_AxisProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


        void OnEnable()
        {
            Is_LeftProp = serializedObject.FindProperty("Is_Left");
            Reference_WheelProp = serializedObject.FindProperty("Reference_Wheel");
            Reference_NameProp = serializedObject.FindProperty("Reference_Name");
            Reference_Parent_NameProp = serializedObject.FindProperty("Reference_Parent_Name");
            Scroll_RateProp = serializedObject.FindProperty("Scroll_Rate");
            Scroll_Y_AxisProp = serializedObject.FindProperty("Scroll_Y_Axis");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
                thisTransform = Selection.activeGameObject.transform;
            }
        }


        public override void OnInspectorGUI()
        {
            bool isPrepared;
            if (Application.isPlaying || thisTransform.parent == null || thisTransform.parent.gameObject.GetComponent<Rigidbody>() == null)
            {
                isPrepared = false;
            }
            else
            {
                isPrepared = true;
            }

            if (isPrepared)
            {
                // Set Inspector window.
                Set_Inspector();
            }
            else
            {
                if (Application.isPlaying)
                {
                    Inspector_In_Runtime();
                }
            }
        }


        void Set_Inspector()
        {
            serializedObject.Update();
            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Track Scroll settings", MessageType.None, true);

            if (GUILayout.Button("Find RoadWheel [ Left ]", GUILayout.Width(200)))
            {
                Is_LeftProp.boolValue = true;
                Find_RoadWheel(Is_LeftProp.boolValue);
            }
            if (GUILayout.Button("Find RoadWheel [ Right ]", GUILayout.Width(200)))
            {
                Is_LeftProp.boolValue = false;
                Find_RoadWheel(Is_LeftProp.boolValue);
            }
            EditorGUILayout.Space();

            Is_LeftProp.boolValue = EditorGUILayout.Toggle("Left", Is_LeftProp.boolValue);
            Reference_WheelProp.objectReferenceValue = EditorGUILayout.ObjectField("Reference Wheel", Reference_WheelProp.objectReferenceValue, typeof(Transform), true);
            if (Reference_WheelProp.objectReferenceValue != null)
            {
                Transform tempTransform = Reference_WheelProp.objectReferenceValue as Transform;
                Reference_NameProp.stringValue = tempTransform.name;
                if (tempTransform.parent)
                {
                    Reference_Parent_NameProp.stringValue = tempTransform.parent.name;
                }
            }
            else
            {
                // Find Reference Wheel with reference to the name.
                string tempName = Reference_NameProp.stringValue;
                string tempParentName = Reference_Parent_NameProp.stringValue;
                if (string.IsNullOrEmpty(tempName) == false && string.IsNullOrEmpty(tempParentName) == false)
                {
                    Reference_WheelProp.objectReferenceValue = thisTransform.parent.Find(tempParentName + "/" + tempName);
                }
            }
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            Reference_NameProp.stringValue = EditorGUILayout.TextField("Reference Name", Reference_NameProp.stringValue);
            Reference_Parent_NameProp.stringValue = EditorGUILayout.TextField("Reference Parent Name", Reference_Parent_NameProp.stringValue);
            GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);

            EditorGUILayout.Slider(Scroll_RateProp, -0.01f, 0.01f, "Scroll Rate");

            Scroll_Y_AxisProp.boolValue = EditorGUILayout.Toggle("Scroll Y Axis", Scroll_Y_AxisProp.boolValue);

            // Update Value
            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
                Find_RoadWheel(Is_LeftProp.boolValue);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }


        void Inspector_In_Runtime()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.7f, 0.7f, 1.0f, 1.0f);
            EditorGUILayout.Slider(Scroll_RateProp, -0.01f, 0.01f, "Scroll Rate");
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }


        void Find_RoadWheel(bool isLeft)
        {
            Transform bodyTransform = thisTransform.parent;
            Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren<Drive_Wheel_CS>();
            float minDist = Mathf.Infinity;
            Transform closestWheel = null;
            foreach (Drive_Wheel_CS driveScript in driveScripts)
            {
                Transform connectedTransform = driveScript.GetComponent<HingeJoint>().connectedBody.transform;
                MeshFilter meshFilter = driveScript.GetComponent<MeshFilter>();
                if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh)
                { // connected to Suspension && not invisible.
                    float tempDist = Vector3.Distance(bodyTransform.position, driveScript.transform.position); // Distance to the MainBody.
                    if (isLeft)
                    { // Left.
                        if (driveScript.transform.localEulerAngles.z == 0.0f)
                        { // Left.
                            if (tempDist < minDist)
                            {
                                closestWheel = driveScript.transform;
                                minDist = tempDist;
                            }
                        }
                    }
                    else
                    { // Right.
                        if (driveScript.transform.localEulerAngles.z != 0.0f)
                        { // Right.
                            if (tempDist < minDist)
                            {
                                closestWheel = driveScript.transform;
                                minDist = tempDist;
                            }
                        }
                    }
                }
            }
            Reference_WheelProp.objectReferenceValue = closestWheel;
        }

    }
}
