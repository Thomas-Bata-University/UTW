using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Bullet_Control_CS))]
	public class Bullet_Control_CSEditor : Editor
	{
		
		SerializedProperty TypeProp;
		SerializedProperty This_TransformProp;
		SerializedProperty This_RigidbodyProp;
		SerializedProperty Impact_ObjectProp;
		SerializedProperty Ricochet_ObjectProp;
		SerializedProperty Explosion_ObjectProp;
		SerializedProperty Explosion_ForceProp;
		SerializedProperty Explosion_RadiusProp;
	
		string[] typeNames = { "AP", "HE" };


		void OnEnable ()
		{
			TypeProp = serializedObject.FindProperty ("Type");
			This_TransformProp = serializedObject.FindProperty ("This_Transform");
			This_RigidbodyProp = serializedObject.FindProperty ("This_Rigidbody");
			Impact_ObjectProp = serializedObject.FindProperty ("Impact_Object");
			Ricochet_ObjectProp = serializedObject.FindProperty ("Ricochet_Object");
			Explosion_ObjectProp = serializedObject.FindProperty ("Explosion_Object");
			Explosion_ForceProp = serializedObject.FindProperty ("Explosion_Force");
			Explosion_RadiusProp = serializedObject.FindProperty ("Explosion_Radius");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox ("Bullet settings", MessageType.None, true);
			This_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField("This Transform", This_TransformProp.objectReferenceValue, typeof(Transform), true);
			This_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField("This Rigidbody", This_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
			EditorGUILayout.Space ();

			TypeProp.intValue = EditorGUILayout.Popup ("Bullet Type", TypeProp.intValue, typeNames);
			EditorGUI.indentLevel++;
			switch (TypeProp.intValue) {
				case 0: // AP
					Impact_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField("Impact Prefab", Impact_ObjectProp.objectReferenceValue, typeof(GameObject), true);
					Ricochet_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField("Ricochet Prefab", Ricochet_ObjectProp.objectReferenceValue, typeof(GameObject), true);
					break;
				case 1: // HE
					Explosion_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField("Explosion Prefab", Explosion_ObjectProp.objectReferenceValue, typeof(GameObject), true);
					EditorGUILayout.Slider(Explosion_ForceProp, 10.0f, 1000000.0f, "Explosion Force");
					EditorGUILayout.Slider(Explosion_RadiusProp, 0.1f, 1000.0f, "Explosion Radius");
					break;
			}
			EditorGUI.indentLevel--;


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}