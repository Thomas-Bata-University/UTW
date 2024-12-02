using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Aiming_Control_CS))]
	public class Aiming_Control_CSEditor : Editor
	{
	
		SerializedProperty OpenFire_AngleProp;


		void OnEnable ()
		{
			OpenFire_AngleProp = serializedObject.FindProperty ("OpenFire_Angle");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Aiming settings", MessageType.None, true);
			EditorGUILayout.Slider (OpenFire_AngleProp, 1.0f, 360.0f, "Open Fire Angle");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}