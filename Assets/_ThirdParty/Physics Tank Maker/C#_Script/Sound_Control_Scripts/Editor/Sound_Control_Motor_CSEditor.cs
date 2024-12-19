using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Sound_Control_Motor_CS))]
	public class Sound_Control_Motor_CSEditor : Editor
	{

		SerializedProperty Max_Motor_VolumeProp;


		void  OnEnable ()
		{
			Max_Motor_VolumeProp = serializedObject.FindProperty ("Max_Motor_Volume");
		}


		public override void  OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
		
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Motor sound settings.", MessageType.None, true);

			EditorGUILayout.Slider (Max_Motor_VolumeProp, 0.1f, 10.0f, "Max Motor Volume");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}

	}

}