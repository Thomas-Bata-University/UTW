using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Menu_Slider_CS))]
	public class Menu_Slider_CSEditor : Editor
	{
		
		SerializedProperty This_SliderProp;
		SerializedProperty Value_TextProp;
		SerializedProperty Key_NameProp;
		SerializedProperty TypeProp;
		SerializedProperty Initial_ValueProp;

		string[] typeNames = { "Attack Multiplier", "Defence Multiplier"};


		void OnEnable ()
		{
			This_SliderProp = serializedObject.FindProperty ("This_Slider");
			Value_TextProp = serializedObject.FindProperty ("Value_Text");
			Key_NameProp = serializedObject.FindProperty ("Key_Name");
			TypeProp = serializedObject.FindProperty ("Type");
			Initial_ValueProp = serializedObject.FindProperty ("Initial_Value");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Menu Slider settings", MessageType.None, true);

			This_SliderProp.objectReferenceValue = EditorGUILayout.ObjectField ("This Slider", This_SliderProp.objectReferenceValue, typeof(Slider), true);
			Value_TextProp.objectReferenceValue = EditorGUILayout.ObjectField ("Value Text", Value_TextProp.objectReferenceValue, typeof(Text), true);
			Key_NameProp.stringValue = EditorGUILayout.TextField ("Key Name", Key_NameProp.stringValue);
			TypeProp.intValue = EditorGUILayout.Popup ("Type", TypeProp.intValue, typeNames);

			if (This_SliderProp.objectReferenceValue) {
				Slider thisSlider = This_SliderProp.objectReferenceValue as Slider;
				EditorGUILayout.Slider (Initial_ValueProp, thisSlider.minValue, thisSlider.maxValue, "Initial Value");
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}