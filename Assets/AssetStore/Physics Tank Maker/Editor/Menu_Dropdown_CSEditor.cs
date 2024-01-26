using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	[ CustomEditor (typeof(Menu_Dropdown_CS))]
	public class Menu_Dropdown_CSEditor : Editor
	{

		SerializedProperty This_DropdownProp;
		SerializedProperty Title_TextProp;
		SerializedProperty Key_NameProp;
		SerializedProperty NumProp;
		SerializedProperty Prefabs_ArrayProp;
		SerializedProperty Default_ValueProp;
		SerializedProperty Symbol_TransformProp;
		SerializedProperty OffsetProp;


		void OnEnable ()
		{
			This_DropdownProp = serializedObject.FindProperty ("This_Dropdown");
			Title_TextProp = serializedObject.FindProperty ("Title_Text");
			Key_NameProp = serializedObject.FindProperty ("Key_Name");
			NumProp = serializedObject.FindProperty ("Num");
			Prefabs_ArrayProp = serializedObject.FindProperty ("Prefabs_Array");
			Default_ValueProp = serializedObject.FindProperty ("Default_Value");
			Symbol_TransformProp = serializedObject.FindProperty ("Symbol_Transform");
			OffsetProp = serializedObject.FindProperty ("Offset");
		}


		public override void OnInspectorGUI ()
		{
			if (Application.isPlaying == false) {
				Set_Inspector ();
			}
		}

		void Set_Inspector ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Dropdown Settings", MessageType.None, true);

			This_DropdownProp.objectReferenceValue = EditorGUILayout.ObjectField ("This Dropdown", This_DropdownProp.objectReferenceValue, typeof(Dropdown), true);
			Title_TextProp.objectReferenceValue = EditorGUILayout.ObjectField ("Title Text", Title_TextProp.objectReferenceValue, typeof(Text), true);
			Key_NameProp.stringValue = EditorGUILayout.TextField ("Key Name", Key_NameProp.stringValue);

			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (NumProp, 1, 64, "Number of Prefabs");
			Prefabs_ArrayProp.arraySize = NumProp.intValue;
			EditorGUI.indentLevel++;
			for (int i = 0; i < NumProp.intValue; i++) {
				Prefabs_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Prefab " + "[" + i + "]", Prefabs_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(GameObject), false);
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Default_ValueProp, 0, Prefabs_ArrayProp.arraySize - 1, "Default Prefab's Index");
			if (Default_ValueProp.intValue > Prefabs_ArrayProp.arraySize - 1) {
				Default_ValueProp.intValue = Prefabs_ArrayProp.arraySize - 1;
			}
			if (Prefabs_ArrayProp.GetArrayElementAtIndex (Default_ValueProp.intValue).objectReferenceValue) {
				EditorGUILayout.HelpBox (Prefabs_ArrayProp.GetArrayElementAtIndex (Default_ValueProp.intValue).objectReferenceValue.name, MessageType.None, false);

			}
			else {
				EditorGUILayout.HelpBox ("! ! ! Empty ! ! !", MessageType.None, false);
			}

			EditorGUILayout.Space ();
			Symbol_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Symbol Object", Symbol_TransformProp.objectReferenceValue, typeof(Transform), true);

			EditorGUILayout.Space ();
			EditorGUILayout.Slider (OffsetProp, -1000.0f, 1000.0f, "Offset");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}



	}
}
