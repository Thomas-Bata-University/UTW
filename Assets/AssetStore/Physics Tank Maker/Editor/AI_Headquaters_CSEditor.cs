using UnityEngine;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(AI_Headquaters_CS))]
	public class AI_Headquaters_CSEditor : Editor
	{
		
		SerializedProperty Order_IntervalProp;
	

		void OnEnable ()
		{
			Order_IntervalProp = serializedObject.FindProperty ("Order_Interval");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("AI order settings", MessageType.None, true);
			EditorGUILayout.Slider (Order_IntervalProp, 0.0f, 10.0f, "Order Interval");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}