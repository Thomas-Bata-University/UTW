using UnityEngine;

namespace ChobiAssets.PTM
{

    public class General_Settings_CS
    {
        // General Function settings.
        public static bool Allow_Instant_Quit = true;
        public static bool Allow_Reload_Scene = true;
        public static bool Allow_Manual_Respawn = true;
        public static bool Allow_Switch_Cursor = false;

        // Input Type settings.
        public static int Input_Type = 1;

        // Camera Rotation settings.
        public static float Camera_Horizontal_Speed = 2.0f;
        public static float Camera_Vertical_Speed = 1.0f;
        public static bool Camera_Invert_Flag = false;
        public static bool Camera_Use_Clamp = true;
        public static bool Camera_Simulate_Head_Physics = false;

        // Camera Avoid Obstacle settings.
        public static float Camera_Avoid_Move_Speed = 30.0f;
        public static float Camera_Avoid_Min_Dist = 2.0f;
        public static float Camera_Avoid_Max_Dist = 30.0f;
        public static float Camera_Avoid_Lag = 0.1f;

        // Aim settings.
        public static bool Use_Auto_Lead = false;
        public static float Aiming_Sensibility = 0.2f;
        public static float Aiming_Offset = 0.1f;

        // Gun Camera settings.
        public static bool Gun_Camera_While_Pressing = false;

        // Scroll Track settings.
        public static string Main_Tex_Name = "_MainTex";


        //
        // Key bindings.
        //

        // General Function Keys settings.
        public static KeyCode Reload_Scene_Key = KeyCode.Tab;
        public static KeyCode Quit_Key = KeyCode.Escape;
        public static KeyCode Switch_Cursor_Key = KeyCode.LeftShift;
        public static KeyCode Pause_Key = KeyCode.P;
        public static KeyCode Respawn_Key = KeyCode.Return;
        public static KeyCode Switch_Canvas_Key = KeyCode.Delete;


        // Tank Select Keys settings.
        public static KeyCode Select_Default_Tank_Key = KeyCode.KeypadEnter;
        public static KeyCode Increase_ID_Key = KeyCode.KeypadPlus;
        public static KeyCode Decrease_ID_Key = KeyCode.KeypadMinus;


        // Drive Keyboard + Mouse settings.
        public static KeyCode Drive_Up_Key = KeyCode.W;
        public static KeyCode Drive_Down_Key = KeyCode.S;
        public static KeyCode Drive_Brake_Key = KeyCode.X;
        public static KeyCode Drive_Left_Key = KeyCode.A;
        public static KeyCode Drive_Right_Key = KeyCode.D;


        // Camera Keyboard + Mouse settings.
        public static KeyCode Camera_Switch_Key = KeyCode.F;
        // Camera GamePad settings.
        public static string Camera_Switch_Pad_Axis = "Vertical3";
        public static float Camera_Switch_Pad_Axis_Direction = 1.0f;
        public static KeyCode Camera_Look_Forward_Pad_Button = KeyCode.Joystick1Button3;
        public static KeyCode Camera_Zoom_In_Pad_Button = KeyCode.Joystick1Button2;
        public static KeyCode Camera_Zoom_Out_Pad_Button = KeyCode.Joystick1Button0;


        // Gun Camera Keyboard + Mouse settings.
        public static KeyCode Gun_Camera_Switch_Key = KeyCode.Mouse1;
        // Gun Camera GamePad settings.
        public static KeyCode Gun_Camera_Switch_Pad_Button = KeyCode.Joystick1Button4;


        // RC Camera Keyboard + Mouse settings.
        public static KeyCode RC_Camera_Switch_Key = KeyCode.LeftControl;
        public static KeyCode RC_Camera_Rotate_Key = KeyCode.Z;
        // RC Camera GamePad settings.
        public static string RC_Camera_Switch_Pad_Axis = "Horizontal3";
        public static float RC_Camera_Switch_Pad_Axis_Direction = -1.0f;


        // Aim Keyboard + Mouse settings.
        public static KeyCode Aim_Mode_Switch_Key = KeyCode.Mouse2;
        public static KeyCode Turret_Cancel_Key = KeyCode.Space;
        public static KeyCode Aim_Lock_On_Left_Key = KeyCode.Q;
        public static KeyCode Aim_Lock_On_Right_Key = KeyCode.E;
        public static KeyCode Aim_Lock_On_Front_Key = KeyCode.Alpha2;
        // Aim GamePad settings.
        public static KeyCode Aim_Mode_Switch_Pad_Button = KeyCode.Joystick1Button9;
        public static KeyCode Turret_Cancel_Pad_Button = KeyCode.Joystick1Button8;
        public static string Aim_Lock_On_Front_Pad_Axis = "Vertical3";
        public static float Aim_Lock_On_Front_Pad_Axis_Direction = -1.0f;


        // Fire Keyboard + Mouse setting.
        public static KeyCode Fire_Key = KeyCode.Mouse0;
        public static KeyCode Switch_Bullet_Key = KeyCode.V;
        // Fire GamePad setting.
        public static KeyCode Fire_Pad_Button = KeyCode.Joystick1Button5;
        public static KeyCode Switch_Bullet_Pad_Button = KeyCode.Joystick1Button6;



    }

}
