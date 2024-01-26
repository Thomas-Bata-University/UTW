using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_ScrollTrack_CS))]
	public class Create_ScrollTrack_CSEditor : Editor
	{

		SerializedProperty Is_LeftProp;
		SerializedProperty Pos_XProp;
		SerializedProperty WidthProp;
		SerializedProperty HeightProp;
		SerializedProperty MatProp;
		SerializedProperty ScaleProp;
		SerializedProperty Line_AProp;
		SerializedProperty Line_BProp;
		SerializedProperty Line_CProp;
		SerializedProperty Line_DProp;
		SerializedProperty Line_EProp;
		SerializedProperty Line_FProp;
		SerializedProperty Line_GProp;
		SerializedProperty Line_HProp;
		SerializedProperty Guide_CountProp;
		SerializedProperty Guide_HeightProp;
		SerializedProperty Guide_PositionsProp;
		SerializedProperty Guide_Line_BottomProp;
		SerializedProperty Guide_Line_TopProp;

		SerializedProperty Position_ArrayProp;
		SerializedProperty Radius_ArrayProp;
		SerializedProperty Start_Angle_ArrayProp;
		SerializedProperty End_Angle_ArrayProp;

		SerializedProperty Show_TextureProp;

        SerializedProperty Saved_MeshProp;

        SerializedProperty Has_ChangedProp;

        Transform thisTransform;
		List <Transform> roadWheelsList;
		List <Transform> supportWheelsList;
		List <Transform> idlerSprocketWheelsList;
		List <CreatingScrollTrackInfo> wheelsInfoList; 
		float snapAngle = 30.0f;
		Mesh mesh;
		int baseCount;


		void OnEnable ()
		{
			Is_LeftProp = serializedObject.FindProperty ("Is_Left");
			Pos_XProp = serializedObject.FindProperty ("Pos_X");
			WidthProp = serializedObject.FindProperty ("Width");
			HeightProp = serializedObject.FindProperty ("Height");
			MatProp = serializedObject.FindProperty ("Mat");
			ScaleProp = serializedObject.FindProperty ("Scale");
			Line_AProp = serializedObject.FindProperty ("Line_A");
			Line_BProp = serializedObject.FindProperty ("Line_B");
			Line_CProp = serializedObject.FindProperty ("Line_C");
			Line_DProp = serializedObject.FindProperty ("Line_D");
			Line_EProp = serializedObject.FindProperty ("Line_E");
			Line_FProp = serializedObject.FindProperty ("Line_F");
			Line_GProp = serializedObject.FindProperty ("Line_G");
			Line_HProp = serializedObject.FindProperty ("Line_H");
			Guide_CountProp = serializedObject.FindProperty ("Guide_Count");
			Guide_HeightProp = serializedObject.FindProperty ("Guide_Height");
			Guide_PositionsProp = serializedObject.FindProperty ("Guide_Positions");
			Guide_Line_BottomProp = serializedObject.FindProperty ("Guide_Line_Bottom");
			Guide_Line_TopProp = serializedObject.FindProperty ("Guide_Line_Top");

			Position_ArrayProp = serializedObject.FindProperty ("Position_Array");
			Radius_ArrayProp = serializedObject.FindProperty ("Radius_Array");
			Start_Angle_ArrayProp = serializedObject.FindProperty ("Start_Angle_Array");
			End_Angle_ArrayProp = serializedObject.FindProperty ("End_Angle_Array");

			Show_TextureProp = serializedObject.FindProperty ("Show_Texture");

            Saved_MeshProp = serializedObject.FindProperty("Saved_Mesh");

            Has_ChangedProp = serializedObject.FindProperty("Has_Changed");

            if (Selection.activeGameObject)
            {
				thisTransform = Selection.activeGameObject.transform;

            }
        }


        public override void OnInspectorGUI ()
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
                // Keep posotion and rotation.
                thisTransform.localPosition = Vector3.zero;
                thisTransform.localEulerAngles = Vector3.zero;

                // Set Inspector window.
                Set_Inspector();
            }
        }


		void Set_Inspector ()
		{
            // Check this is the root of the prefab or not.
            if (PrefabUtility.IsAnyPrefabInstanceRoot(Selection.activeGameObject))
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("We need to unpack this prefab to create 'Scroll_Track'.", MessageType.Warning, true);
                if (GUILayout.Button("Unpack Prefab", GUILayout.Width(200)))
                {
                    PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                return;
            }

            // Check the Prefab Mode.
            if (PrefabUtility.IsPartOfPrefabInstance(thisTransform))
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("\n'Scroll_Track' can be created only in the Prefab Mode.\nPlease go to the Prefab Mode, or Unpack the prefab.\n", MessageType.Warning, true);
                return;
            }

            serializedObject.Update ();

            GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUI.BeginChangeCheck ();

			// Main Settings.
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Basic Settings", MessageType.None, true);
			EditorGUILayout.Space ();

            Show_Message();
            EditorGUILayout.Space ();

			// Get Default Settings
			if (GUILayout.Button ("Get Default Settings", GUILayout.Width (200))) {
				Get_Default_Settings ();
			}
			EditorGUILayout.Space ();
			Is_LeftProp.boolValue = EditorGUILayout.Toggle ("Left", Is_LeftProp.boolValue);
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Pos_XProp, 0.0f, 10.0f, "Position X");
			EditorGUILayout.Slider (WidthProp, 0.1f, 3.0f, "Width");
			EditorGUILayout.Slider (HeightProp, 0.01f, 0.5f, "Thickness");
			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Guide_CountProp, 0, 4, "Number of Guides");
			if (Guide_CountProp.intValue != 0) {
				EditorGUI.indentLevel++;
				EditorGUILayout.Slider (Guide_HeightProp, 0.01f, 0.5f, "Guide Height");
				Guide_PositionsProp.arraySize = Guide_CountProp.intValue;
				for (int i = 0; i < Guide_CountProp.intValue; i++) {
					EditorGUILayout.Slider (Guide_PositionsProp.GetArrayElementAtIndex (i), -WidthProp.floatValue / 2.0f, WidthProp.floatValue / 2.0f, "Guide [" + i + "] Position");
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			// Shape Settings.
			EditorGUILayout.HelpBox ("Shape Settings", MessageType.None, true);
            EditorGUI.indentLevel++;
            for (int i = 0; i < Radius_ArrayProp.arraySize; i++) {
				EditorGUILayout.HelpBox ("Wheel [" + i + "]", MessageType.None, true);
				EditorGUILayout.Slider (Radius_ArrayProp.GetArrayElementAtIndex (i), 0.0f, 1.0f, "Radius");
				EditorGUILayout.Slider (Start_Angle_ArrayProp.GetArrayElementAtIndex (i), -360.0f, 360.0f, "Start Angle");
				EditorGUILayout.Slider (End_Angle_ArrayProp.GetArrayElementAtIndex (i), -360.0f, 360.0f, "End Angle");
				Start_Angle_ArrayProp.GetArrayElementAtIndex (i).floatValue = Mathf.Ceil (Start_Angle_ArrayProp.GetArrayElementAtIndex (i).floatValue / snapAngle) * snapAngle;
				End_Angle_ArrayProp.GetArrayElementAtIndex (i).floatValue = Mathf.Ceil (End_Angle_ArrayProp.GetArrayElementAtIndex (i).floatValue / snapAngle) * snapAngle;
				EditorGUILayout.Space ();
			}
            EditorGUI.indentLevel--;

            // UV Settings.
            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("UV Settings", MessageType.None, true);
			MatProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material", MatProp.objectReferenceValue, typeof(Material), false);
			if (MatProp.objectReferenceValue == null) {
				MeshRenderer thisRenderer = thisTransform.GetComponent <MeshRenderer> ();
				if (thisRenderer) {
					MatProp.objectReferenceValue = thisRenderer.sharedMaterial as Material;
				}
			}
			EditorGUILayout.Space ();
			if (EditorGUI.EndChangeCheck ()) {
				Create ();
			}
			Show_TextureProp.boolValue = EditorGUILayout.Toggle ("Show Texture", Show_TextureProp.boolValue);
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (ScaleProp, -2.0f, 2.0f, "Scale");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Line_AProp, 0.0f, 1.0f, "Line A (Inner of Inside)");
			EditorGUILayout.Slider (Line_BProp, 0.0f, 1.0f, "Line B (Outer of Inside)");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Line_CProp, 0.0f, 1.0f, "Line C (Inner of Outer-Surface)");
			EditorGUILayout.Slider (Line_DProp, 0.0f, 1.0f, "Line D (Outer of Outer-Surface)");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Line_EProp, 0.0f, 1.0f, "Line E (Outer of Outside)");
			EditorGUILayout.Slider (Line_FProp, 0.0f, 1.0f, "Line F (Inner of Outside)");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Line_GProp, 0.0f, 1.0f, "Line G (Outer of Inner-Surface)");
			EditorGUILayout.Slider (Line_HProp, 0.0f, 1.0f, "Line H (Inner of Inner-Surface)");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (Guide_CountProp.intValue != 0) {
				EditorGUILayout.Slider (Guide_Line_TopProp, 0.0f, 1.0f, "Line (Top of Guide)");
				EditorGUILayout.Slider (Guide_Line_BottomProp, 0.0f, 1.0f, "Line (Bottom of Guide)");
			}

			if (EditorGUI.EndChangeCheck ()) {
				Create ();
			}

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Show_Message();

            EditorGUILayout.Space();
            GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			if (GUILayout.Button ("Save as a New Mesh", GUILayout.Width (200))) {
				Create_New_Mesh ();
			}
			EditorGUILayout.Space ();

            if (Saved_MeshProp.objectReferenceValue)
            {
                if (GUILayout.Button("Overwrite Saved Mesh", GUILayout.Width(200)))
                {
                    Overwrite_Saved_Mesh();
                }
            }
            EditorGUI.indentLevel++;
            Saved_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField("Saved Mesh", Saved_MeshProp.objectReferenceValue, typeof(Mesh), false);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                Has_ChangedProp.boolValue = !Has_ChangedProp.boolValue;
            }

            if (Event.current.commandName == "UndoRedoPerformed")
            {
                Debug.LogWarning("Please save the mesh again after using Undo or Redo.");
                Create();
            }

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}


        void Show_Message()
        {
            MeshFilter thisMeshFilter = thisTransform.GetComponent<MeshFilter>();
            if (thisMeshFilter && thisMeshFilter.sharedMesh)
            {
                if (string.IsNullOrEmpty(thisMeshFilter.sharedMesh.name))
                {
                    GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                    EditorGUILayout.HelpBox("The mesh is not saved yet.", MessageType.None, true);
                    GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
                }
                else
                {
                    EditorGUILayout.HelpBox("The mesh is saved as '" + thisMeshFilter.sharedMesh.name + "'", MessageType.None, true);
                }
            }
            else
            {
                GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
                EditorGUILayout.HelpBox("The mesh is not created yet.", MessageType.None, true);
                GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            }
        }


        void Create()
        {
            // Check the wheels.
            if (Radius_ArrayProp.arraySize == 0)
            {
                return;
            }

            // Set MeshFilter and MeshRenderer.
            MeshFilter thisFilter = thisTransform.GetComponent<MeshFilter>();
            if (thisFilter == null)
            {
                thisFilter = thisTransform.gameObject.AddComponent<MeshFilter>();
            }
            MeshRenderer thisRenderer = thisTransform.GetComponent<MeshRenderer>();
            if (thisRenderer == null)
            {
                thisRenderer = thisTransform.gameObject.AddComponent<MeshRenderer>();
            }
            thisRenderer.material = MatProp.objectReferenceValue as Material;

            // Create Mesh.
            mesh = new Mesh();
            Set_Vertices();
            Set_Triangles();
            Set_UV();
            mesh.RecalculateNormals();

            //TangentSolver (ref mesh);
            Set_Tangent();
            thisFilter.sharedMesh = mesh;
        }


        void Set_Vertices()
        {
            List<Vector3> verticesList = new List<Vector3>();
            float xSign;
            if (Is_LeftProp.boolValue)
            { // Left
                xSign = -1.0f;
            }
            else
            { // Right
                xSign = 1.0f;
            }

            // Outer Line of Inner-Surface.
            for (int i = 0; i < Position_ArrayProp.arraySize; i++)
            {
                float deltaAngle = Mathf.Abs(Start_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue - End_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue);
                float tempSign = Mathf.Sign(End_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue - Start_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue);
                int cornerCount = (int)(deltaAngle / snapAngle + 1.0f);
                for (int j = 0; j < cornerCount; j++)
                {
                    float radius = Radius_ArrayProp.GetArrayElementAtIndex(i).floatValue;
                    Vector3 tempPos;
                    tempPos.x = (Pos_XProp.floatValue * xSign) + ((WidthProp.floatValue / 2.0f) * xSign);
                    tempPos.x += thisTransform.position.x * xSign;
                    tempPos.y = Mathf.Sin(Mathf.Deg2Rad * (90.0f + Start_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue + (snapAngle * j * tempSign)));
                    tempPos.y *= radius;
                    tempPos.y += Position_ArrayProp.GetArrayElementAtIndex(i).vector3Value.y;
                    tempPos.y -= thisTransform.position.y;
                    tempPos.z = Mathf.Cos(Mathf.Deg2Rad * (90.0f + Start_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue + (snapAngle * j * tempSign)));
                    tempPos.z *= radius;
                    tempPos.z += Position_ArrayProp.GetArrayElementAtIndex(i).vector3Value.z;
                    tempPos.z -= thisTransform.position.z;
                    verticesList.Add(tempPos);
                }
            }
            verticesList.Add(verticesList[0]); // Last one.
            baseCount = verticesList.Count;

            // Inner Line of Inner-Surface.
            for (int i = 0; i < baseCount; i++)
            {
                Vector3 tempPos = verticesList[i];
                tempPos.x -= WidthProp.floatValue * xSign;
                verticesList.Add(tempPos);
            }

            // Outer Line of Outer-Surface.
            for (int i = 0; i < baseCount - 1; i++)
            {
                Vector3 previousPos;
                if (i == 0)
                {
                    previousPos = verticesList[baseCount - 2];
                }
                else
                {
                    previousPos = verticesList[i - 1];
                }
                Vector3 currentPos = verticesList[i];
                Vector3 nextPos = verticesList[i + 1];
                float previousAng = Mathf.Atan2(previousPos.y - currentPos.y, previousPos.z - currentPos.z) * Mathf.Rad2Deg;
                float nextAng = Mathf.Atan2(nextPos.y - currentPos.y, nextPos.z - currentPos.z) * Mathf.Rad2Deg;
                if (previousAng > nextAng)
                {
                    nextAng = 360.0f + nextAng;
                }
                float targetAng = (nextAng + previousAng) / 2.0f;
                Vector3 tempPos;
                tempPos.x = (Pos_XProp.floatValue * xSign) + ((WidthProp.floatValue / 2.0f) * xSign);
                tempPos.x += thisTransform.position.x * xSign;
                tempPos.y = Mathf.Sin(Mathf.Deg2Rad * targetAng) * HeightProp.floatValue;
                tempPos.y += currentPos.y;
                tempPos.z = Mathf.Cos(Mathf.Deg2Rad * targetAng) * HeightProp.floatValue;
                tempPos.z += currentPos.z;
                verticesList.Add(tempPos);
            }
            verticesList.Add(verticesList[baseCount * 2]); // Last one.

            // Inner Line of Outer-Surface.
            for (int i = 0; i < baseCount; i++)
            {
                Vector3 tempPos = verticesList[(baseCount * 2) + i];
                tempPos.x -= WidthProp.floatValue * xSign;
                verticesList.Add(tempPos);
            }

            // Outer Side.
            for (int i = 0; i < baseCount; i++)
            {
                verticesList.Add(verticesList[i]);
            }
            for (int i = 0; i < baseCount; i++)
            {
                verticesList.Add(verticesList[(baseCount * 2) + i]);
            }

            // Inner Side.
            for (int i = 0; i < baseCount; i++)
            {
                verticesList.Add(verticesList[baseCount + i]);
            }
            for (int i = 0; i < baseCount; i++)
            {
                verticesList.Add(verticesList[(baseCount * 3) + i]);
            }

            // Center-Guide
            for (int i = 0; i < Guide_CountProp.intValue; i++)
            {
                for (int j = 0; j < baseCount; j++)
                {
                    Vector3 tempPos = verticesList[j];
                    tempPos.x = (Pos_XProp.floatValue * xSign) + (Guide_PositionsProp.GetArrayElementAtIndex(i).floatValue * xSign);
                    tempPos.x += thisTransform.position.x * xSign;
                    verticesList.Add(tempPos);
                }
                for (int j = 0; j < baseCount - 1; j++)
                {
                    Vector3 previousPos;
                    if (j == 0)
                    {
                        previousPos = verticesList[baseCount - 2];
                    }
                    else
                    {
                        previousPos = verticesList[j - 1];
                    }
                    Vector3 currentPos = verticesList[j];
                    Vector3 nextPos = verticesList[j + 1];
                    float previousAng = Mathf.Atan2(previousPos.y - currentPos.y, previousPos.z - currentPos.z) * Mathf.Rad2Deg;
                    float nextAng = Mathf.Atan2(nextPos.y - currentPos.y, nextPos.z - currentPos.z) * Mathf.Rad2Deg;
                    if (previousAng > nextAng)
                    {
                        nextAng = 360.0f + nextAng;
                    }
                    float targetAng = (nextAng + previousAng) / 2.0f;
                    Vector3 tempPos;
                    tempPos.x = (Pos_XProp.floatValue * xSign) + (Guide_PositionsProp.GetArrayElementAtIndex(i).floatValue * xSign);
                    tempPos.x += thisTransform.position.x * xSign;
                    tempPos.y = Mathf.Sin(Mathf.Deg2Rad * targetAng) * -Guide_HeightProp.floatValue;
                    tempPos.y += currentPos.y;
                    tempPos.z = Mathf.Cos(Mathf.Deg2Rad * targetAng) * -Guide_HeightProp.floatValue;
                    tempPos.z += currentPos.z;
                    verticesList.Add(tempPos);
                }
                verticesList.Add(verticesList[baseCount * (9 + (i * 2))]); // Last one.
            }

            //	Set Vertices.
            mesh.vertices = verticesList.ToArray();
        }


        void Set_Triangles()
        {
            List<int> trianglesList = new List<int>();

            // Inner-Surface.
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add(i);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add(i + 1);
                    trianglesList.Add(baseCount + i);
                }
                else
                {
                    trianglesList.Add(baseCount + i);
                    trianglesList.Add(i + 1);
                }
            }
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add(baseCount + i);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add(i + 1);
                    trianglesList.Add(baseCount + i + 1);
                }
                else
                {
                    trianglesList.Add(baseCount + i + 1);
                    trianglesList.Add(i + 1);
                }
            }

            // Outer-Surface.
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 2) + i);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 3) + i);
                    trianglesList.Add((baseCount * 2) + i + 1);
                }
                else
                {
                    trianglesList.Add((baseCount * 2) + i + 1);
                    trianglesList.Add((baseCount * 3) + i);
                }
            }
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 2) + i + 1);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 3) + i);
                    trianglesList.Add((baseCount * 3) + i + 1);
                }
                else
                {
                    trianglesList.Add((baseCount * 3) + i + 1);
                    trianglesList.Add((baseCount * 3) + i);
                }
            }

            // Outer Side.
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 4) + i);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 5) + i);
                    trianglesList.Add((baseCount * 4) + i + 1);
                }
                else
                {
                    trianglesList.Add((baseCount * 4) + i + 1);
                    trianglesList.Add((baseCount * 5) + i);
                }
            }
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 4) + i + 1);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 5) + i);
                    trianglesList.Add((baseCount * 5) + i + 1);
                }
                else
                {
                    trianglesList.Add((baseCount * 5) + i + 1);
                    trianglesList.Add((baseCount * 5) + i);
                }
            }

            // Inner Side.
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 6) + i);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 6) + i + 1);
                    trianglesList.Add((baseCount * 7) + i);
                }
                else
                {
                    trianglesList.Add((baseCount * 7) + i);
                    trianglesList.Add((baseCount * 6) + i + 1);
                }
            }
            for (int i = 0; i < baseCount - 1; i++)
            {
                trianglesList.Add((baseCount * 6) + i + 1);
                if (Is_LeftProp.boolValue)
                {
                    trianglesList.Add((baseCount * 7) + i + 1);
                    trianglesList.Add((baseCount * 7) + i);
                }
                else
                {
                    trianglesList.Add((baseCount * 7) + i);
                    trianglesList.Add((baseCount * 7) + i + 1);
                }
            }

            // Center-Guide
            for (int i = 0; i < Guide_CountProp.intValue; i++)
            {
                for (int j = 0; j < baseCount - 1; j++)
                {
                    trianglesList.Add((baseCount * (8 + (i * 2))) + j);
                    if (Is_LeftProp.boolValue)
                    {
                        trianglesList.Add((baseCount * (8 + (i * 2))) + j + 1);
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j);
                    }
                    else
                    {
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j);
                        trianglesList.Add((baseCount * (8 + (i * 2))) + j + 1);
                    }
                }
                for (int j = 0; j < baseCount - 1; j++)
                {
                    trianglesList.Add((baseCount * (8 + (i * 2))) + j + 1);
                    if (Is_LeftProp.boolValue)
                    {
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j + 1);
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j);
                    }
                    else
                    {
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j);
                        trianglesList.Add((baseCount * (9 + (i * 2))) + j + 1);
                    }
                }
            }

            // Set Triangles.
            mesh.triangles = trianglesList.ToArray();
        }


        void Set_UV()
        {
            List<Vector2> uvList = new List<Vector2>();
            List<float> vList = new List<float>() {
                Line_GProp.floatValue,
                Line_HProp.floatValue,
                Line_DProp.floatValue,
                Line_CProp.floatValue,
                Line_FProp.floatValue,
                Line_EProp.floatValue,
                Line_AProp.floatValue,
                Line_BProp.floatValue,
            };

            for (int i = 0; i < Guide_CountProp.intValue; i++)
            {
                vList.Add(Guide_Line_BottomProp.floatValue);
                vList.Add(Guide_Line_TopProp.floatValue);
            }

            for (int i = 0; i < vList.Count; i++)
            {
                float currentU = 0.0f;
                for (int j = 0; j < baseCount; j++)
                {
                    float dist;
                    if (j == 0)
                    {
                        dist = 0.0f;
                    }
                    else
                    {
                        dist = Vector3.Distance(mesh.vertices[j - 1], mesh.vertices[j]);
                    }
                    currentU += dist * ScaleProp.floatValue;
                    Vector2 tempUV = new Vector2(currentU, vList[i]);
                    uvList.Add(tempUV);
                }
            }

            // Set UV.
            mesh.uv = uvList.ToArray();
        }


        void Set_Tangent()
        {
            List<Vector4> tangentList = new List<Vector4>();
            float tempW;
            if (Is_LeftProp.boolValue)
            {
                tempW = -1.0f;
            }
            else
            {
                tempW = 1.0f;
            }

            // Inner-Surface
            for (int i = 0; i < (baseCount * 2); i++)
            {
                Vector3 tempTangent = Vector3.Cross(mesh.normals[i], Vector3.left);
                tangentList.Add(new Vector4(tempTangent.x, tempTangent.y, tempTangent.z, tempW));
            }

            // Outer-Surface
            for (int i = (baseCount * 2); i < (baseCount * 4); i++)
            {
                Vector3 tempTangent = Vector3.Cross(mesh.normals[i], Vector3.right);
                tangentList.Add(new Vector4(tempTangent.x, tempTangent.y, tempTangent.z, tempW));
            }

            // Outer Side
            for (int i = (baseCount * 4); i < (baseCount * 5); i++)
            {
                tangentList.Add(tangentList[i - (baseCount * 4)]);
            }
            for (int i = (baseCount * 5); i < (baseCount * 6); i++)
            {
                tangentList.Add(tangentList[i - (baseCount * 3)]);
            }

            // Inner Side
            for (int i = (baseCount * 6); i < (baseCount * 7); i++)
            {
                tangentList.Add(tangentList[i - (baseCount * 5)]);
            }
            for (int i = (baseCount * 7); i < (baseCount * 8); i++)
            {
                tangentList.Add(tangentList[i - (baseCount * 4)]);
            }

            // Center-Guide
            for (int i = 0; i < Guide_CountProp.intValue; i++)
            {
                for (int j = (baseCount * (8 + (i * 2))); j < (baseCount * (10 + (i * 2))); j++)
                {
                    tangentList.Add(tangentList[j - (baseCount * (8 + (i * 2)))]);
                }
            }

            // Set Tangents.
            mesh.SetTangents(tangentList);
        }


        void Get_Default_Settings()
        {
            Create_RoadWheelsList();
            Create_SupportWheelsList();
            Create_IdlerSprocketWheelsList();
            Create_WheelsInfoList();
            Set_Arrays();
        }


        void Create_RoadWheelsList()
        {
            // Find RoadWheels.
            roadWheelsList = new List<Transform>();
            List<Transform> tempList = new List<Transform>();
            Transform bodyTransform = thisTransform.parent;
            Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren<Drive_Wheel_CS>();
            foreach (Drive_Wheel_CS driveScript in driveScripts)
            {
                Transform connectedTransform = driveScript.GetComponent<HingeJoint>().connectedBody.transform;
                MeshFilter meshFilter = driveScript.GetComponent<MeshFilter>();
                if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh && driveScript.transform.localEulerAngles.z == 0.0f)
                { // connected to Suspension && visible && Left.
                    tempList.Add(driveScript.transform);
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
        }


        void Create_SupportWheelsList()
        {
            // Find SupportWheels.
            supportWheelsList = new List<Transform>();
            List<Transform> tempList = new List<Transform>();
            Create_SupportWheel_CS[] supportWheelScripts = thisTransform.parent.GetComponentsInChildren<Create_SupportWheel_CS>();
            foreach (Create_SupportWheel_CS supportWheelScript in supportWheelScripts)
            {
                MeshFilter[] tempFilters = supportWheelScript.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter tempFilter in tempFilters)
                {
                    if (tempFilter.gameObject.layer == Layer_Settings_CS.Wheels_Layer && tempFilter.transform.localEulerAngles.z == 0.0f && tempFilter.sharedMesh)
                    { // Wheel's layer && Left.
                        tempList.Add(tempFilter.transform);
                    }
                }
            }

            // Sort (front >> rear)
            int tempCount = tempList.Count;
            for (int i = 0; i < tempCount; i++)
            {
                float minPosZ = -Mathf.Infinity;
                int targetIndex = 0;
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j].position.z > minPosZ)
                    {
                        minPosZ = tempList[j].position.z;
                        targetIndex = j;
                    }
                }
                supportWheelsList.Add(tempList[targetIndex]);
                tempList.RemoveAt(targetIndex);
            }
        }


        void Create_IdlerSprocketWheelsList()
        {
            idlerSprocketWheelsList = new List<Transform>();
            List<Transform> tempList = new List<Transform>();

            // Find IdlerWheels.
            Create_IdlerWheel_CS idlerWheelScript = thisTransform.parent.GetComponentInChildren<Create_IdlerWheel_CS>();
            MeshFilter[] tempFilters = idlerWheelScript.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter tempFilter in tempFilters)
            {
                if (tempFilter.gameObject.layer == Layer_Settings_CS.Wheels_Layer && tempFilter.transform.localEulerAngles.z == 0.0f && tempFilter.sharedMesh)
                { // Wheel's layer && Left.
                    tempList.Add(tempFilter.transform);
                    break;
                }
            }

            // Find SprocketWheels.
            Create_SprocketWheel_CS sprocketWheelScript = thisTransform.parent.GetComponentInChildren<Create_SprocketWheel_CS>();
            tempFilters = sprocketWheelScript.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter tempFilter in tempFilters)
            {
                if (tempFilter.gameObject.layer == Layer_Settings_CS.Wheels_Layer && tempFilter.transform.localEulerAngles.z == 0.0f && tempFilter.sharedMesh)
                { // Wheel's layer && Left.
                    tempList.Add(tempFilter.transform);
                    Pos_XProp.floatValue = -tempFilter.transform.position.x;
                    break;
                }
            }

            // Sort (front >> rear)
            int tempCount = tempList.Count;
            if (tempCount != 2)
            {
                Debug.LogError("SprocketWheel and IdlerWheel cannot be found.");
                return;
            }
            for (int i = 0; i < tempCount; i++)
            {
                float minPosZ = -Mathf.Infinity;
                int targetIndex = 0;
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j].position.z > minPosZ)
                    {
                        minPosZ = tempList[j].position.z;
                        targetIndex = j;
                    }
                }
                idlerSprocketWheelsList.Add(tempList[targetIndex]);
                tempList.RemoveAt(targetIndex);
            }
        }


        void Create_WheelsInfoList()
        {
            wheelsInfoList = new List<CreatingScrollTrackInfo>();

            // Add RoadWheels.
            for (int i = 0; i < roadWheelsList.Count; i++)
            {
                CreatingScrollTrackInfo tempInfo = new CreatingScrollTrackInfo();
                tempInfo.position = roadWheelsList[i].position;
                tempInfo.radius = roadWheelsList[i].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
                tempInfo.startAngle = 180.0f;
                tempInfo.endAngle = 180.0f;
                wheelsInfoList.Add(tempInfo);
            }

            // Add Front Wheel (Sprocket or Idler).
            CreatingScrollTrackInfo tempFrontInfo = new CreatingScrollTrackInfo();
            tempFrontInfo.position = idlerSprocketWheelsList[0].position;
            tempFrontInfo.radius = idlerSprocketWheelsList[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
            tempFrontInfo.startAngle = -180.0f;
            tempFrontInfo.endAngle = 0.0f;
            wheelsInfoList.Add(tempFrontInfo);

            // Add SupportWheels.
            if (supportWheelsList.Count != 0)
            { // This tank has SupportWheels.
                for (int i = 0; i < supportWheelsList.Count; i++)
                {
                    CreatingScrollTrackInfo tempInfo = new CreatingScrollTrackInfo();
                    tempInfo.position = supportWheelsList[i].position;
                    tempInfo.radius = supportWheelsList[i].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
                    tempInfo.startAngle = 0.0f;
                    tempInfo.endAngle = 0.0f;
                    wheelsInfoList.Add(tempInfo);
                }
            }
            else
            { // No SupportWheel >> Add RoadWheels (front >> rear).
                for (int i = (roadWheelsList.Count - 1); i >= 0; i--)
                {
                    CreatingScrollTrackInfo tempInfo = new CreatingScrollTrackInfo();
                    tempInfo.position = roadWheelsList[i].position;
                    tempInfo.radius = roadWheelsList[i].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
                    tempInfo.startAngle = 0.0f;
                    tempInfo.endAngle = 0.0f;
                    wheelsInfoList.Add(tempInfo);
                }
            }

            // Add Rear Wheel (Sprocket or Idler).
            CreatingScrollTrackInfo tempRearInfo = new CreatingScrollTrackInfo();
            tempRearInfo.position = idlerSprocketWheelsList[1].position;
            tempRearInfo.radius = idlerSprocketWheelsList[1].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
            tempRearInfo.startAngle = 0.0f;
            tempRearInfo.endAngle = 180.0f;
            wheelsInfoList.Add(tempRearInfo);
        }


        void Set_Arrays()
        {
            Position_ArrayProp.arraySize = wheelsInfoList.Count;
            Radius_ArrayProp.arraySize = wheelsInfoList.Count;
            Start_Angle_ArrayProp.arraySize = wheelsInfoList.Count;
            End_Angle_ArrayProp.arraySize = wheelsInfoList.Count;
            for (int i = 0; i < wheelsInfoList.Count; i++)
            {
                Position_ArrayProp.GetArrayElementAtIndex(i).vector3Value = wheelsInfoList[i].position;
                Radius_ArrayProp.GetArrayElementAtIndex(i).floatValue = wheelsInfoList[i].radius;
                Start_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue = wheelsInfoList[i].startAngle;
                End_Angle_ArrayProp.GetArrayElementAtIndex(i).floatValue = wheelsInfoList[i].endAngle;
            }
        }


        void Create_New_Mesh()
        {
            // Check the Prefab Mode.
            if (PrefabUtility.IsPartOfPrefabInstance(thisTransform))
            {
                Debug.LogWarning("The ScrollTrack can be created only in the Prefab Mode. Please go to the Prefab Mode, or Unpack the prefab.");
                return;
            }

            Create();

            // Check the mesh.
            MeshFilter thisMeshFilter = thisTransform.GetComponent<MeshFilter>();
            Mesh tempMesh = thisMeshFilter.sharedMesh;
            if (tempMesh == null)
            {
                Debug.LogWarning("Mesh does not created yet.");
                return;
            }

            // Check the "User" folder.
            if (Directory.Exists("Assets/Physics Tank Maker/User/") == false)
            {
                AssetDatabase.CreateFolder("Assets/Physics Tank Maker", "User");
            }

            // Create a new path.
            string newPath;
            if (Is_LeftProp.boolValue)
            { // Left
                newPath = "Assets/Physics Tank Maker/User/" + thisTransform.root.name + "_ScrollTrack_L " + DateTime.Now.ToString("yyMMdd_HHmmss") + ".asset";
            }
            else
            { // Right
                newPath = "Assets/Physics Tank Maker/User/" + thisTransform.root.name + "_ScrollTrack_R " + DateTime.Now.ToString("yyMMdd_HHmmss") + ".asset";
            }

            // Create the new asset.
            AssetDatabase.CreateAsset(tempMesh, newPath);

            // Set the new mesh to the MeshFilter.
            Saved_MeshProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath(newPath, typeof(Mesh)) as Mesh;
            thisMeshFilter.sharedMesh = Saved_MeshProp.objectReferenceValue as Mesh;

            Debug.Log("New mesh has been created in 'User' folder.");
        }


        void Overwrite_Saved_Mesh()
        {
            // Check the Prefab Mode.
            if (PrefabUtility.IsPartOfPrefabInstance(thisTransform))
            {
                Debug.LogWarning("The ScrollTrack can be created only in the Prefab Mode. Please go to the Prefab Mode, or Unpack the prefab.");
                return;
            }

            Create();

            // Check the mesh.
            MeshFilter thisMeshFilter = thisTransform.GetComponent<MeshFilter>();
            Mesh tempMesh = thisMeshFilter.sharedMesh;
            if (tempMesh == null)
            {
                Debug.LogWarning("Mesh does not created yet.");
                return;
            }

            // Get the path of the "Saved_Mesh".
            string savedMeshPath = AssetDatabase.GetAssetPath(Saved_MeshProp.objectReferenceValue);

            // Overwrite the "Saved_Mesh".
            AssetDatabase.CreateAsset(tempMesh, savedMeshPath);

            // Set the new mesh to the MeshFilter.
            Saved_MeshProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath(savedMeshPath, typeof(Mesh)) as Mesh;
            thisMeshFilter.sharedMesh = Saved_MeshProp.objectReferenceValue as Mesh;

            Debug.Log("'Saved Mesh' has been overwritten.");
        }

    }

}