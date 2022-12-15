using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource Audio;
    public AudioClip Click;
    public AudioClip Bear;
    public AudioClip Chicken;
    public AudioClip Rabbit;
    public AudioClip Gem;
    public AudioClip RightFood;
    public AudioClip WrongFood;

    public AudioClip Eat;
    public AudioClip Eagle;
    public AudioClip Hit;
    public AudioClip FoxHit;


    public static SFXManager sfx_instance;

    private void Awake() {
        if(sfx_instance != null && sfx_instance != this) {
            Destroy(this.gameObject);
            return;
        }

        sfx_instance = this;
        DontDestroyOnLoad(this);
    }
}
