using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_Text_Control_CS))]
	public class UI_Text_Control_CSEditor : Editor
	{
		
		SerializedProperty Fade_In_TimeProp;
		SerializedProperty Fade_Out_TimeProp;


		void OnEnable ()
		{
			Fade_In_TimeProp = serializedObject.FindProperty ("Fade_In_Time");
			Fade_Out_TimeProp = serializedObject.FindProperty ("Fade_Out_Time");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Fading settings", MessageType.None, true);
			EditorGUILayout.Slider (Fade_In_TimeProp, 0.1f, 10.0f, "Fade In Time");
			EditorGUILayout.Slider (Fade_Out_TimeProp, 0.1f, 10.0f, "Fade Out Time");


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}