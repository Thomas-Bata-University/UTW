using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_Input_01_Mouse_CS : Cannon_Fire_Input_00_Base_CS
	{

        protected Turret_Horizontal_CS turretScript;


        public override void Prepare(Cannon_Fire_CS cannonFireScript)
        {
            this.cannonFireScript = cannonFireScript;

            turretScript = GetComponentInParent<Turret_Horizontal_CS>();
        }


        public override void Get_Input()
		{
            
            // Fire.
            if (turretScript.Is_Ready && Input.GetKey(General_Settings_CS.Fire_Key))
            {
                cannonFireScript.Fire();
            }

            // Switch the bullet type.
            if (Input.GetKeyDown(General_Settings_CS.Switch_Bullet_Key))
            {
                // Call the "Bullet_Generator_CS" scripts.
                for (int i = 0; i < cannonFireScript.Bullet_Generator_Scripts.Length; i++)
                {
                    if (cannonFireScript.Bullet_Generator_Scripts[i] == null)
                    {
                        continue;
                    }
                    cannonFireScript.Bullet_Generator_Scripts[i].Switch_Bullet_Type();
                }

                // Reload.
                cannonFireScript.StartCoroutine("Reload");
            }
        }

	}

}
