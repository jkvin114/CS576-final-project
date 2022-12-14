using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSFX : MonoBehaviour
{
    public AudioSource Audio;
    public AudioClip Click;
    public static ClickSFX click_instance;

    private void Awake() {
        if(click_instance != null && click_instance != this) {
            Destroy(this.gameObject);
            return;
        }

        click_instance = this;
        DontDestroyOnLoad(this);
    }
}
