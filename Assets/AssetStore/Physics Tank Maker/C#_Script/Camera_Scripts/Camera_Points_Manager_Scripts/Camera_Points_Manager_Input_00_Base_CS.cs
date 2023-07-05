using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Camera_Points_Manager_Input_00_Base_CS : MonoBehaviour
	{

		protected Camera_Points_Manager_CS managerScript;


		public virtual void Prepare(Camera_Points_Manager_CS managerScript)
		{
			this.managerScript = managerScript;
		}


		public virtual void Get_Input()
		{
		}

	}

}
