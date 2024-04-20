using UnityEngine;

public class ObserverController : PlayerController {
    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    protected override void Start() {
        tankPosition = TankPositions.OBSERVER;
        Debug.Log($"Active position {tankPosition} | owner {Owner.ClientId}");
    }

    private void Update() {
        MouseLook(100f, 100f, 80f);
        Move();
    }

    protected override void Move() {

    }

}//END
