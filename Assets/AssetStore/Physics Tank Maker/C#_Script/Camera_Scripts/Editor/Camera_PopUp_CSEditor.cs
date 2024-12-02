using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Camera_PopUp_CS))]
	public class Camera_PopUp_CS_CSEditor : Editor
	{
		
		SerializedProperty Main_CameraProp;
		SerializedProperty PopUp_LengthProp;
		SerializedProperty PopUp_Start_FOVProp;
		SerializedProperty PopUp_Goal_FOVProp;
		SerializedProperty Motion_CurveProp;


		void OnEnable ()
		{
			Main_CameraProp = serializedObject.FindProperty ("Main_Camera");
			PopUp_LengthProp = serializedObject.FindProperty ("PopUp_Length");
			PopUp_Start_FOVProp = serializedObject.FindProperty ("PopUp_Start_FOV");
			PopUp_Goal_FOVProp = serializedObject.FindProperty ("PopUp_Goal_FOV");
			Motion_CurveProp = serializedObject.FindProperty ("Motion_Curve");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Camera Pop Up settings", MessageType.None, true);
			Main_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Main Camera", Main_CameraProp.objectReferenceValue, typeof(Camera), true);
			EditorGUILayout.Slider (PopUp_LengthProp, 0.1f, 10.0f, "Pop Up Length");
			EditorGUILayout.Slider (PopUp_Start_FOVProp, 1.0f, 100.0f, "Pop Up Start FOV");
			EditorGUILayout.Slider (PopUp_Goal_FOVProp, 1.0f, 100.0f, "Pop Up Goal FOV");
			Motion_CurveProp.animationCurveValue = EditorGUILayout.CurveField ("Pop Up Motion", Motion_CurveProp.animationCurveValue, Color.red, new Rect (Vector2.zero, new Vector2 (1.0f, 1.0f)));


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}