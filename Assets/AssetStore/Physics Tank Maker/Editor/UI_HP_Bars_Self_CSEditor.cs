using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_HP_Bars_Self_CS))]
	public class UI_HP_Bars_Self_CSEditor : Editor
	{
		
		SerializedProperty This_CanvasProp;
		SerializedProperty Bars_Parent_TransformProp;
		SerializedProperty Body_BarProp;
		SerializedProperty Turret_BarProp;
		SerializedProperty Left_Track_BarProp;
		SerializedProperty Right_Track_BarProp;
		SerializedProperty Flash_TimeProp;


		void OnEnable ()
		{
			This_CanvasProp = serializedObject.FindProperty ("This_Canvas");
			Bars_Parent_TransformProp = serializedObject.FindProperty ("Bars_Parent_Transform");
			Body_BarProp = serializedObject.FindProperty ("Body_Bar");
			Turret_BarProp = serializedObject.FindProperty ("Turret_Bar");
			Left_Track_BarProp = serializedObject.FindProperty ("Left_Track_Bar");
			Right_Track_BarProp = serializedObject.FindProperty ("Right_Track_Bar");
			Flash_TimeProp = serializedObject.FindProperty ("Flash_Time");
		}


		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying) {
				return;
			}

			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Self Hit Points Bars settings", MessageType.None, true);

			This_CanvasProp.objectReferenceValue = EditorGUILayout.ObjectField ("This Canvas", This_CanvasProp.objectReferenceValue, typeof(Canvas), true);
			Bars_Parent_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Bars Parent", Bars_Parent_TransformProp.objectReferenceValue, typeof(Transform), true);
			Body_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Body Bar", Body_BarProp.objectReferenceValue, typeof(Image), true);
			Turret_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Turret Bar", Turret_BarProp.objectReferenceValue, typeof(Image), true);
			Left_Track_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Track Bar", Left_Track_BarProp.objectReferenceValue, typeof(Image), true);
			Right_Track_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Track Bar", Right_Track_BarProp.objectReferenceValue, typeof(Image), true);
			EditorGUILayout.Space ();

			EditorGUILayout.Slider (Flash_TimeProp, 0.1f, 5.0f, "Flash Time");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}