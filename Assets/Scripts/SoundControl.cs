using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour {

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    public List<AudioSource> audio;

    public void EnableSound() {
        foreach (AudioSource audioSource in audio) {
            audioSource.enabled = true;
            audioSource.Play();
        }
    }

} //END