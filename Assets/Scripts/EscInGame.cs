using UnityEngine;

public class EscInGame : MonoBehaviour
{
    public GameObject canvas;
    
    // TODO: Make this into an event
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvas != null)
            {
                canvas.SetActive(!canvas.activeSelf);
            } else Debug.LogError("Canvas doesnt exist!");

            if (Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }
    }
}
