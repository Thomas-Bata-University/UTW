using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{
	[ CustomEditor (typeof(Recoil_Brake_CS))]
	public class Recoil_Brake_CSEditor : Editor
	{
	
		SerializedProperty Total_TimeProp;
		SerializedProperty Recoil_LengthProp;
		SerializedProperty Motion_CurveProp;

		void OnEnable ()
		{
			Total_TimeProp = serializedObject.FindProperty ("Total_Time");
			Recoil_LengthProp = serializedObject.FindProperty ("Recoil_Length");
			Motion_CurveProp = serializedObject.FindProperty ("Motion_Curve");
		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Recoil Brake settings", MessageType.None, true);
			EditorGUILayout.Slider (Total_TimeProp, 0.01f, 10.0f, "Recoil Time");
			EditorGUILayout.Slider (Recoil_LengthProp, 0.0f, 10.0f, "Length");
			Motion_CurveProp.animationCurveValue = EditorGUILayout.CurveField ("Motion", Motion_CurveProp.animationCurveValue, Color.red, new Rect (Vector2.zero, new Vector2 (1.0f, 1.0f)));
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}