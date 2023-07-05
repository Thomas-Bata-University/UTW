using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Damage_Control_09_Armor_Collider_CS))]
	public class Damage_Control_09_Armor_Collider_CSEditor : Editor
	{
	
		SerializedProperty Damage_MultiplierProp;


		void OnEnable ()
		{
			Damage_MultiplierProp = serializedObject.FindProperty ("Damage_Multiplier");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Damage Multiplier settings", MessageType.None, true);
			EditorGUILayout.Slider (Damage_MultiplierProp, 0.0f, 10.0f, "Damage Multiplier");


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}

	}

}