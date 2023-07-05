using UnityEngine;

namespace ChobiAssets.PTS
{
    public class PTS_Layer_Settings_CS
    {
        const int maxLayersNum = 11;
        public const int Wheels_Layer = 9; // for wheels.
        public const int Reinforce_Layer = 10; // for suspension arms and track reinforce objects. (Ignore all the collision)
        public const int Body_Layer = 11; // for MainBody.


        public const int Layer_Mask = ~((1 << 2) + (1 << Reinforce_Layer)); // Layer 2 = Ignore Ray
        public const int Aiming_Layer_Mask = ~((1 << 2) + (1 << Wheels_Layer) + (1 << Reinforce_Layer));
        public const int Anti_Slipping_Layer_Mask = ~((1 << 2) + (1 << Reinforce_Layer) + (1 << Body_Layer));


        public static void Layers_Collision_Settings()
        {
            // Wheels Layer settings.
            Physics.IgnoreLayerCollision(Wheels_Layer, Wheels_Layer, true); // Wheels ignore each other.
            Physics.IgnoreLayerCollision(Wheels_Layer, Body_Layer, true); // Wheels ignore MainBody.


            // Reinforce Layer settings.
            for (int i = 0; i <= maxLayersNum; i++)
            {
                Physics.IgnoreLayerCollision(Reinforce_Layer, i, true); // Suspension arms and track reinforce objects ignore all.
            }
        }

    }

}
