using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(ReticleWheel_Control_CS))]
	public class ReticleWheel_Control_CSEditor : Editor
	{

		SerializedProperty ReticleWheel_NameProp;
		SerializedProperty Gun_CameraProp;
		SerializedProperty SpeedProp;
		SerializedProperty Max_DistanceProp;
		SerializedProperty MultiplierProp;
	

		void OnEnable ()
		{
			ReticleWheel_NameProp = serializedObject.FindProperty ("ReticleWheel_Name");
			Gun_CameraProp = serializedObject.FindProperty ("Gun_Camera");
			SpeedProp = serializedObject.FindProperty ("Speed");
			Max_DistanceProp = serializedObject.FindProperty ("Max_Distance");
			MultiplierProp = serializedObject.FindProperty ("Multiplier");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Reticle settings", MessageType.None, true);
			ReticleWheel_NameProp.stringValue = EditorGUILayout.TextField ("Reticle Wheel Name", ReticleWheel_NameProp.stringValue);
			Gun_CameraProp.objectReferenceValue = EditorGUILayout.ObjectField ("Gun Camera", Gun_CameraProp.objectReferenceValue, typeof(Camera), true);
			EditorGUILayout.Slider (SpeedProp, 100.0f, 5000.0f, "Speed");
			EditorGUILayout.Slider (Max_DistanceProp, 100.0f, 10000.0f, "Max Distance");
			EditorGUILayout.Slider (MultiplierProp, 0.1f, 10.0f, "Multiplier");


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}