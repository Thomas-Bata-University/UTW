using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Damage_Control_00_Base_CS : MonoBehaviour
	{

		protected Damage_Control_Center_CS centerScript;


        protected virtual void Start()
		{
			centerScript = GetComponentInParent <Damage_Control_Center_CS>();
		}


		public virtual bool Get_Damage(float damage, int bulletType)
		{
			return false;
		}

	}

}
