using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Fade_Control_CS))]
	public class Fade_Control_CSEditor : Editor
	{
		
		SerializedProperty Fade_ImageProp;
		SerializedProperty Fade_TimeProp;


		void OnEnable ()
		{
			Fade_ImageProp = serializedObject.FindProperty ("Fade_Image");
			Fade_TimeProp = serializedObject.FindProperty ("Fade_Time");
		}


		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Fading settings", MessageType.None, true);
			Fade_ImageProp.objectReferenceValue = EditorGUILayout.ObjectField ("Fade Image", Fade_ImageProp.objectReferenceValue, typeof(Image), true);
			EditorGUILayout.Slider (Fade_TimeProp, 0.1f, 10.0f, "Fade Time");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}