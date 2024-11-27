using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(UI_HP_Bars_Target_CS))]
	public class UI_HP_Bars_Target_CSEditor : Editor
	{

		SerializedProperty This_CanvasProp;
		SerializedProperty This_Canvas_ScalerProp;
		SerializedProperty Bars_Parent_TransformProp;
		SerializedProperty Body_BarProp;
		SerializedProperty Turret_BarProp;
		SerializedProperty Left_Track_BarProp;
		SerializedProperty Right_Track_BarProp;
		SerializedProperty Flash_TimeProp;
        SerializedProperty Normal_AlphaProp;
        SerializedProperty Friend_ColorProp;
        SerializedProperty Hostile_ColorProp;


        void OnEnable ()
		{
			This_CanvasProp = serializedObject.FindProperty ("This_Canvas");
			This_Canvas_ScalerProp = serializedObject.FindProperty ("This_Canvas_Scaler");
			Bars_Parent_TransformProp = serializedObject.FindProperty ("Bars_Parent_Transform");
			Body_BarProp = serializedObject.FindProperty ("Body_Bar");
			Turret_BarProp = serializedObject.FindProperty ("Turret_Bar");
			Left_Track_BarProp = serializedObject.FindProperty ("Left_Track_Bar");
			Right_Track_BarProp = serializedObject.FindProperty ("Right_Track_Bar");
			Flash_TimeProp = serializedObject.FindProperty ("Flash_Time");
            Normal_AlphaProp = serializedObject.FindProperty("Normal_Alpha");
            Friend_ColorProp = serializedObject.FindProperty("Friend_Color");
            Hostile_ColorProp = serializedObject.FindProperty("Hostile_Color");
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
			This_Canvas_ScalerProp.objectReferenceValue = EditorGUILayout.ObjectField ("This Canvas Scaler", This_Canvas_ScalerProp.objectReferenceValue, typeof(CanvasScaler), true);
			Bars_Parent_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Bars Parent", Bars_Parent_TransformProp.objectReferenceValue, typeof(Transform), true);
			Body_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Body Bar", Body_BarProp.objectReferenceValue, typeof(Image), true);
			Turret_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Turret Bar", Turret_BarProp.objectReferenceValue, typeof(Image), true);
			Left_Track_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Track Bar", Left_Track_BarProp.objectReferenceValue, typeof(Image), true);
			Right_Track_BarProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Track Bar", Right_Track_BarProp.objectReferenceValue, typeof(Image), true);
			EditorGUILayout.Space ();

			EditorGUILayout.Slider (Flash_TimeProp, 0.1f, 5.0f, "Flash Time");
            EditorGUILayout.Slider(Normal_AlphaProp, 0.1f, 1.0f, "Normal Alpha");
            Friend_ColorProp.colorValue = EditorGUILayout.ColorField("Friend Color", Friend_ColorProp.colorValue);
            Hostile_ColorProp.colorValue = EditorGUILayout.ColorField("Hostile Color", Hostile_ColorProp.colorValue);

            EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}