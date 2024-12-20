using UnityEngine;

namespace ChobiAssets.PTS
{
	
	public class PTS_Static_Track_Setting_CS : MonoBehaviour
	{

		public bool Use_2ndPiece;

		Transform frontTransform;
		Transform rearTransform;

		int type;
		string anchorName;
		string anchorParentName;

		Transform thisTransform;
		Rigidbody bodyRigidbody;
		float count;

		void Start ()
		{
			thisTransform = transform;
			Transform parentTransform = thisTransform.parent;
			string baseName = this.name.Substring (0, 12); // e.g. "TrackBelt_L_"
			int thisNum = int.Parse (this.name.Substring (12)); // e.g. "1"
			// Find front piece.
			frontTransform = parentTransform.Find (baseName + (thisNum + 1)); // Find a piece having next number.
			if (frontTransform == null) { // It must be the last piece.
				frontTransform = parentTransform.Find (baseName + 1); // The 1st piece.
			}
			// Find rear piece.
			rearTransform = parentTransform.Find (baseName + (thisNum - 1)); // Find a piece having previous number.
			if (rearTransform == null) { // It must be the 1st piece.
				rearTransform = parentTransform.Find (baseName + (transform.parent.childCount / 2)); // The last piece.
			}
			// Find MainBody's Rigidbody.
			bodyRigidbody = parentTransform.parent.GetComponent <Rigidbody> ();
		}
			
		void Update ()
		{
			if (thisTransform.parent) {
				Set_Type ();
				if (bodyRigidbody.velocity.magnitude < 0.1f) {
					count += Time.deltaTime;
					if (count > 1.0f) {
						Time.timeScale = 0.0f;
					}
				}

			} else { // Tracks may be broken.
				Destroy (this);
			}
		}

		void Set_Type ()
		{
			// Detect RoadWheel.
			Collider[] hitColliders = Physics.OverlapSphere (thisTransform.position, 0.1f, PTS_Layer_Settings_CS.Layer_Mask);
			foreach (Collider hitCollider in hitColliders) {
				Transform tempParent = hitCollider.transform.parent;
				if (tempParent) {
					if (tempParent.GetComponent <PTS_Create_RoadWheel_CS> ()) {
						type = 1; // Anchor type
						anchorName = hitCollider.transform.name;
						anchorParentName = hitCollider.transform.parent.name;
						return;
					} else if (tempParent.GetComponent <PTS_Create_SprocketWheel_CS> () || tempParent.GetComponent <PTS_Create_IdlerWheel_CS> () || tempParent.GetComponent <PTS_Create_SupportWheel_CS> ()) {
						type = 0; // Static type
						anchorName = null;
						anchorParentName = null;
						return;
					}
				}
			}
			// cannot detect any wheel.
			type = 2; // Dynamic type
			anchorName = null;
			anchorParentName = null;
		}

		void Set_Child_Script ()
		{ // Called from "Create_TrackBelt_CSEditor".
			// Add "Static_Track_CS" and set values, and disable it.
			PTS_Static_Track_CS trackScript = gameObject.AddComponent < PTS_Static_Track_CS > ();
			trackScript.Front_Transform = frontTransform;
			trackScript.Rear_Transform = rearTransform;
			switch (type) {
			case 0: // Static
				if (frontTransform.GetComponent < PTS_Static_Track_Setting_CS > ().type == 1) { // The front piece is Anchor type.
					trackScript.Type = 2; // >> Dynamic
				} else if (rearTransform.GetComponent < PTS_Static_Track_Setting_CS > ().type == 1) { // The rear piece is Anchor type.
					trackScript.Type = 2; // >> Dynamic
				} else {
					trackScript.Type = 0; // Static
				}
				break;
			case 1: // Anchor
				trackScript.Type = 1; // Anchor
				trackScript.Anchor_Name = anchorName;
				trackScript.Anchor_Parent_Name = anchorParentName;
				break;
			case 2: // Dynamic
				trackScript.Type = 2; // Dynamic
				break;
			}
			// Add "Static_Track_Switch_Mesh_CS".
			if (Use_2ndPiece) {
				gameObject.AddComponent < PTS_Static_Track_Switch_Mesh_CS > ();
			}
			// Remove needless components.
			HingeJoint hingeJoint = GetComponent < HingeJoint > ();
			if (hingeJoint) {
				DestroyImmediate (hingeJoint);
			}
			Rigidbody rigidbody = GetComponent < Rigidbody > ();
			if (rigidbody) {
				DestroyImmediate (rigidbody);
			}
			PTS_Stabilizer_CS stabilizerScript = GetComponent < PTS_Stabilizer_CS > ();
			if (stabilizerScript) {
				DestroyImmediate (stabilizerScript);
			}
			// Disable CapsuleCollider.
			CapsuleCollider [] capsuleColliders = GetComponents <CapsuleCollider> ();
			for (int i = 0; i < capsuleColliders.Length; i++) {
				capsuleColliders [i].enabled = false;
			}
			// Remove child objects such as Reinforce piece.
			for (int i = 0; i < transform.childCount; i++) {
				GameObject childObject = transform.GetChild (i).gameObject;
				if (childObject.layer == PTS_Layer_Settings_CS.Reinforce_Layer) { // Reinforce.
					DestroyImmediate (childObject);
				}
			}
		}

		void Remove_Setting_Script ()
		{ // Called from "Create_TrackBelt_CSEditor".
			DestroyImmediate (this);
		}

	}

}