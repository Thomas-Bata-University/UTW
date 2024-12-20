﻿using UnityEngine;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_Input_03_For_Triggers_Drive_CS : Cannon_Fire_Input_00_Base_CS
	{

		public override void Get_Input()
		{
            // Fire.
            if (Input.GetButton("Fire2"))
            {
                cannonFireScript.Fire();
            }

            // Switch the bullet type.
            if (Input.GetKeyDown(General_Settings_CS.Switch_Bullet_Pad_Button))
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
