using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM BGM_instance;
    public AudioClip BGM2;
    public AudioClip BGM3;
    public AudioClip BGM4; 
    public AudioClip BGM_SNOW;
    public AudioClip BGM_DARK;
    public AudioClip BGM_DESERT;
    public AudioClip BGM_PLAIN;
    public AudioClip BGM_RIVER;
    public AudioClip BGM_SPOOKY;
    private void Awake() {
        if(BGM_instance != null && BGM_instance != this) {
            Destroy(this.gameObject);
            return;
        }
        
        BGM_instance = this;
        DontDestroyOnLoad(this);
    }
    public void onStart()
    {

        GetComponent<AudioSource>().Stop();

        GetComponent<AudioSource>().clip = BGM_PLAIN;
        GetComponent<AudioSource>().Play();
    }
    public void onRunCountUp(int num)
    {
        AudioClip clip=null;
        if (num == 1)
        {
            
           clip = BGM_DESERT;
            
        }

        if (num == 2)
        {
            clip = BGM_DARK;
        }
        if (num == 4)
        {
            clip = BGM2;
        }
        if (num == 6)
        {
            clip = BGM3;
        }
        if (num == 8)
        {

            clip = BGM_SNOW;
        }
        if (num == 10)
        {

            clip = BGM_SPOOKY;
        }
        if (num == 12)
        {
            clip = BGM4;
        }

        if (clip != null)
        {
            GetComponent<AudioSource>().Stop();

            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
    }
}
