using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Sound_Control_Impact_CS))]
	public class Sound_Control_Impact_CSEditor : Editor
	{

		SerializedProperty Min_VariationProp;
		SerializedProperty Max_VariationProp;
		SerializedProperty Min_PitchProp;
		SerializedProperty Max_PitchProp;
		SerializedProperty Min_VolumeProp;
		SerializedProperty Max_VolumeProp;
        SerializedProperty IntervalProp;


        void OnEnable ()
		{
			Min_VariationProp = serializedObject.FindProperty ("Min_Variation");
			Max_VariationProp = serializedObject.FindProperty ("Max_Variation");
			Min_PitchProp = serializedObject.FindProperty ("Min_Pitch");
			Max_PitchProp = serializedObject.FindProperty ("Max_Pitch");
			Min_VolumeProp = serializedObject.FindProperty ("Min_Volume");
            Max_VolumeProp = serializedObject.FindProperty("Max_Volume");
            IntervalProp = serializedObject.FindProperty ("Interval");
		}


		public override void  OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
		
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Impact sound settings.", MessageType.None, true);

			EditorGUILayout.Slider (Min_VariationProp, 0.001f, 0.1f, "Min Variation");
			EditorGUILayout.Slider (Max_VariationProp, 0.001f, 0.1f, "Max Variation");
			EditorGUILayout.Slider (Min_PitchProp, 0.1f, 10.0f, "Min Pitch");
			EditorGUILayout.Slider (Max_PitchProp, 0.1f, 10.0f, "Max Pitch");
			EditorGUILayout.Slider (Min_VolumeProp, 0.1f, 10.0f, "Min Volume");
			EditorGUILayout.Slider (Max_VolumeProp, 0.1f, 10.0f, "Max Volume");
            EditorGUILayout.Slider (IntervalProp, 0.0f, 1.0f, "Interval");

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}

	}

}