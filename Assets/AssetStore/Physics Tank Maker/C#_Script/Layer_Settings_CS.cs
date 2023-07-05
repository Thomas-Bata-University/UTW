using UnityEngine;

namespace ChobiAssets.PTM
{

    public class Layer_Settings_CS
    {
        const int maxLayersNum = 14;
        public const int Wheels_Layer = 9; // for wheels.
        public const int Reinforce_Layer = 10; // for suspension arms and track reinforce objects. (Ignore all the collision)
        public const int Body_Layer = 11; // for MainBody.
        public const int Bullet_Layer = 12; // for bullet.
        public const int Armor_Collider_Layer = 13; // for "Armor_Collider" and "Track_Collider".
        public const int Extra_Collider_Layer = 14; // for Extra Collier.

        // Layer Mask settings.
        public const int Layer_Mask = ~((1 << 2) + (1 << Reinforce_Layer) + (1 << Bullet_Layer) + (1 << Extra_Collider_Layer)); // Ignore "Layer 2(Ignore Ray)", "Reinforce_Layer", "Bullet_Layer", "Extra_Collider_Layer".
        public const int Aiming_Layer_Mask = ~((1 << 2) + (1 << Wheels_Layer) + (1 << Reinforce_Layer) + (1 << Bullet_Layer) + (1 << Extra_Collider_Layer)); // Ignore "Layer 2(Ignore Ray)", "Wheels_Layer", "Reinforce_Layer", "Bullet_Layer", "Extra_Collider_Layer".
        public const int Anti_Slipping_Layer_Mask = ~((1 << 2) + (1 << Reinforce_Layer) + (1 << Body_Layer) + (1 << Extra_Collider_Layer)); // Ignore "Layer 2(Ignore Ray)", "Reinforce_Layer", "Body_Layer", "Extra_Collider_Layer".
        public const int Detect_Body_Layer_Mask = 1 << Body_Layer; // Hit only "Body_Layer". (Used for detecting a tank)


        public static void Layers_Collision_Settings()
        {
            // "Wheels_Layer" settings.
            Physics.IgnoreLayerCollision(Wheels_Layer, Wheels_Layer, true); // Wheels ignore each other.
            Physics.IgnoreLayerCollision(Wheels_Layer, Body_Layer, true); // Wheels ignore MainBody.


            // "Reinforce_Layer" settings.
            for (int i = 0; i <= maxLayersNum; i++)
            {
                Physics.IgnoreLayerCollision(Reinforce_Layer, i, true); // Suspension arms and track reinforce objects ignore all.
            }

            // "Bullet_Layer" settings.
            Physics.IgnoreLayerCollision(Bullet_Layer, Bullet_Layer, true); // Bullets ignore each other.
            Physics.IgnoreLayerCollision(Bullet_Layer, Wheels_Layer, true); // Bullets ignore any wheels.

            // "Armor_Collider_Layer" settings.
            for (int i = 0; i <= maxLayersNum; i++)
            {
                Physics.IgnoreLayerCollision(Armor_Collider_Layer, i, true);
            }
            Physics.IgnoreLayerCollision(Armor_Collider_Layer, Bullet_Layer, false); // "Armor_colliders" collide with only bullets.

            // "Extra_Collider_Layer" settings.
            for (int i = 0; i <= maxLayersNum; i++)
            {
                Physics.IgnoreLayerCollision(Extra_Collider_Layer, i, true);
            }
            Physics.IgnoreLayerCollision(Extra_Collider_Layer, Extra_Collider_Layer, false); // Extra colliders collide with only each other.
        }

    }

}
