using UnityEngine;

public class ObserverController : PlayerController {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private void Update() {
        MouseLook(5f, 100f, 80f);
    }

}//END