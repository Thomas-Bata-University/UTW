using UnityEngine;

namespace ChobiAssets.PTS
{
	public class PTS_Static_Track_Switch_Mesh_CS : MonoBehaviour
	{
		
		PTS_Static_Track_CS thisTrackScript;
		PTS_Static_Track_CS parentScript;
		MeshFilter thisMeshFilter;
		Mesh mesh_A;
		Mesh mesh_B;
		bool isLeft;
		Mesh currentMesh;

		void Start ()
		{ // Do not change to "Awake ()". Because "Awake ()" is called while creating the Static_Track.
			thisTrackScript = GetComponent <PTS_Static_Track_CS> ();
			thisMeshFilter = GetComponent <MeshFilter> ();
			mesh_A = thisMeshFilter.mesh;
			mesh_B = thisTrackScript.Front_Transform.GetComponent <MeshFilter> ().mesh;
			currentMesh = mesh_A;
			// Set direction.
			if (transform.localPosition.y > 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
		}
	
		void LateUpdate ()
		{
			if (thisTrackScript.enabled) {
				if (isLeft) { // Left
					if (parentScript.Switch_Mesh_L) {
						currentMesh = mesh_A;
					} else {
						currentMesh = mesh_B;
					}
					thisMeshFilter.mesh = currentMesh;
				} else { // Right
					if (parentScript.Switch_Mesh_R) {
						currentMesh = mesh_A;
					} else {
						currentMesh = mesh_B;
					}
					thisMeshFilter.mesh = currentMesh;
				}
			}
		}

		void Prepare_With_Static_Track (PTS_Static_Track_CS tempScript)
		{ // Called from parent's "PTS_Static_Track_CS".
			parentScript = tempScript;
		}

	}
}
