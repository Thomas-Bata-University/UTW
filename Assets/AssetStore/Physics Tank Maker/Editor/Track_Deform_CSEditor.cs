using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

    [CustomEditor(typeof(Track_Deform_CS))]
    public class Track_Deform_CSEditor : Editor
    {

        SerializedProperty Anchor_NumProp;
        SerializedProperty Anchor_ArrayProp;
        SerializedProperty Anchor_NamesProp;
        SerializedProperty Anchor_Parent_NamesProp;
        SerializedProperty Width_ArrayProp;
        SerializedProperty Height_ArrayProp;
        SerializedProperty Offset_ArrayProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;


        void OnEnable()
        {
            Anchor_NumProp = serializedObject.FindProperty("Anchor_Num");
            Anchor_ArrayProp = serializedObject.FindProperty("Anchor_Array");
            Anchor_NamesProp = serializedObject.FindProperty("Anchor_Names");
            Anchor_Parent_NamesProp = serializedObject.FindProperty("Anchor_Parent_Names");
            Width_ArrayProp = serializedObject.FindProperty("Width_Array");
            Height_ArrayProp = serializedObject.FindProperty("Height_Array");
            Offset_ArrayProp = serializedObject.FindProperty("Offset_Array");

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
        }


        void Set_Inspector()
        {
            if (EditorApplication.isPlaying == false)
            {
                serializedObject.Update();

                GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);

                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("Track Deform settings", MessageType.None, true);

                if (GUILayout.Button("Find RoadWheels [ Left ]", GUILayout.Width(200)))
                {
                    Find_RoadWheels(true);
                }
                if (GUILayout.Button("Find RoadWheels [ Right ]", GUILayout.Width(200)))
                {
                    Find_RoadWheels(false);
                }
                EditorGUILayout.Space();

                EditorGUILayout.IntSlider(Anchor_NumProp, 1, 64, "Number of Anchor Wheels");
                EditorGUILayout.Space();
                Anchor_ArrayProp.arraySize = Anchor_NumProp.intValue;
                Anchor_NamesProp.arraySize = Anchor_NumProp.intValue;
                Anchor_Parent_NamesProp.arraySize = Anchor_NumProp.intValue;
                Width_ArrayProp.arraySize = Anchor_NumProp.intValue;
                Height_ArrayProp.arraySize = Anchor_NumProp.intValue;
                Offset_ArrayProp.arraySize = Anchor_NumProp.intValue;
                for (int i = 0; i < Anchor_ArrayProp.arraySize; i++)
                {
                    Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField("Anchor Wheel", Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue, typeof(Transform), true);
                    if (Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
                        Transform tempTransform = Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                        Anchor_NamesProp.GetArrayElementAtIndex(i).stringValue = tempTransform.name;
                        if (tempTransform.parent)
                        {
                            Anchor_Parent_NamesProp.GetArrayElementAtIndex(i).stringValue = tempTransform.parent.name;
                        }
                    }
                    else
                    {
                        // Find Anchor with reference to the name.
                        string temp_AnchorName = Anchor_NamesProp.GetArrayElementAtIndex(i).stringValue;
                        string temp_AnchorParentName = Anchor_Parent_NamesProp.GetArrayElementAtIndex(i).stringValue;
                        if (string.IsNullOrEmpty(temp_AnchorName) == false && string.IsNullOrEmpty(temp_AnchorParentName) == false)
                        {
                            Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue = thisTransform.transform.parent.Find(temp_AnchorParentName + "/" + temp_AnchorName);
                        }
                    }
                    EditorGUILayout.Space();
                    GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
                    Anchor_NamesProp.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextField("Anchor Name", Anchor_NamesProp.GetArrayElementAtIndex(i).stringValue);
                    Anchor_Parent_NamesProp.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextField("Anchor Parent Name", Anchor_Parent_NamesProp.GetArrayElementAtIndex(i).stringValue);
                    GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
                    EditorGUILayout.Slider(Width_ArrayProp.GetArrayElementAtIndex(i), 0.1f, 10.0f, "Width");
                    if (Width_ArrayProp.GetArrayElementAtIndex(i).floatValue == 0.0f)
                    {
                        Width_ArrayProp.GetArrayElementAtIndex(i).floatValue = 0.5f;
                    }
                    EditorGUILayout.Slider(Height_ArrayProp.GetArrayElementAtIndex(i), 0.1f, 10.0f, "Height");
                    if (Height_ArrayProp.GetArrayElementAtIndex(i).floatValue == 0.0f)
                    {
                        Height_ArrayProp.GetArrayElementAtIndex(i).floatValue = 1.0f;
                    }
                    EditorGUILayout.Slider(Offset_ArrayProp.GetArrayElementAtIndex(i), -10.0f, 10.0f, "Offset");
                    EditorGUILayout.Space();
                }

                // Update Value
                if (GUI.changed || GUILayout.Button("Update Values") || Event.current.commandName == "UndoRedoPerformed")
                {
                    Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
                    Set_Vertices();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
            }
        }


        void Set_Vertices()
        {
            if (thisTransform.GetComponent<MeshFilter>().sharedMesh == null)
            {
                Debug.LogError("Mesh is not assigned in the Mesh Filter.");
                return;
            }
            Mesh thisMesh = thisTransform.GetComponent<MeshFilter>().sharedMesh;
            float[] initialPosArray = new float[Anchor_ArrayProp.arraySize];
            IntArray[] movableVerticesList = new IntArray[Anchor_ArrayProp.arraySize];
            // Get vertices in the range.
            for (int i = 0; i < Anchor_ArrayProp.arraySize; i++)
            {
                if (Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    Transform anchorTransform = Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                    initialPosArray[i] = anchorTransform.localPosition.x;
                    Vector3 anchorPos = thisTransform.transform.InverseTransformPoint(anchorTransform.position);
                    List<int> withinVerticesList = new List<int>();
                    for (int j = 0; j < thisMesh.vertices.Length; j++)
                    {
                        float distZ = Mathf.Abs(anchorPos.z - thisMesh.vertices[j].z);
                        float distY = Mathf.Abs((anchorPos.y + Offset_ArrayProp.GetArrayElementAtIndex(i).floatValue) - thisMesh.vertices[j].y);
                        if (distZ <= Width_ArrayProp.GetArrayElementAtIndex(i).floatValue * 0.5f && distY <= Height_ArrayProp.GetArrayElementAtIndex(i).floatValue * 0.5f)
                        {
                            withinVerticesList.Add(j);
                        }
                    }
                    IntArray withinVerticesArray = new IntArray(withinVerticesList.ToArray());
                    movableVerticesList[i] = withinVerticesArray;
                }
            }
            // Set values.
            EditorApplication.delayCall += () =>
            {
                Track_Deform_CS deformScript = thisTransform.GetComponent<Track_Deform_CS>();
                deformScript.Initial_Pos_Array = initialPosArray;
                deformScript.Initial_Vertices = thisMesh.vertices;
                deformScript.Movable_Vertices_List = movableVerticesList;
            };
        }


        void Find_RoadWheels(bool isLeft)
        {
            // Find RoadWheels.
            List<Transform> roadWheelsList = new List<Transform>();
            List<Transform> tempList = new List<Transform>();
            Transform bodyTransform = thisTransform.parent;
            Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren<Drive_Wheel_CS>();
            foreach (Drive_Wheel_CS driveScript in driveScripts)
            {
                Transform connectedTransform = driveScript.GetComponent<HingeJoint>().connectedBody.transform;
                MeshFilter meshFilter = driveScript.GetComponent<MeshFilter>();
                if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh)
                { // connected to Suspension && not invisible.
                    if (isLeft)
                    { // Left.
                        if (driveScript.transform.localEulerAngles.z == 0.0f)
                        { // Left.
                            tempList.Add(driveScript.transform);
                        }
                    }
                    else
                    { // Right.
                        if (driveScript.transform.localEulerAngles.z != 0.0f)
                        { // Right.
                            tempList.Add(driveScript.transform);
                        }
                    }
                }
            }
            // Sort (rear >> front)
            int tempCount = tempList.Count;
            for (int i = 0; i < tempCount; i++)
            {
                float maxPosZ = Mathf.Infinity;
                int targetIndex = 0;
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j].position.z < maxPosZ)
                    {
                        maxPosZ = tempList[j].position.z;
                        targetIndex = j;
                    }
                }
                roadWheelsList.Add(tempList[targetIndex]);
                tempList.RemoveAt(targetIndex);
            }
            // Set
            Anchor_NumProp.intValue = roadWheelsList.Count;
            Anchor_ArrayProp.arraySize = Anchor_NumProp.intValue;
            for (int i = 0; i < Anchor_ArrayProp.arraySize; i++)
            {
                Anchor_ArrayProp.GetArrayElementAtIndex(i).objectReferenceValue = roadWheelsList[i];
            }
        }

    }

}