namespace ChobiAssets.PTM
{

    public class Steer_Wheel_Input_99_AI_CS : Steer_Wheel_Input_00_Base_CS
    {

        AI_CS aiScript;


        public override void Prepare(Steer_Wheel_CS steerScript)
        {
            this.steerScript = steerScript;

            aiScript = transform.root.GetComponentInChildren<AI_CS>();
        }


        public override void Get_Input()
        {
            steerScript.Horizontal_Input = aiScript.Turn_Order;
        }

    }

}
