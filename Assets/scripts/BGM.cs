using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM BGM_instance;
    public AudioClip BGM2;
    public AudioClip BGM3;
    public AudioClip BGM4;
    private void Awake() {
        if(BGM_instance != null && BGM_instance != this) {
            Destroy(this.gameObject);
            return;
        }
        
        BGM_instance = this;
        DontDestroyOnLoad(this);
    }
    public void onRunCountUp(int num)
    {
        if (num == 1)
        {

            GetComponent<AudioSource>().Stop();

            GetComponent<AudioSource>().clip = BGM2;
            GetComponent<AudioSource>().Play();
        }
        if (num == 3)
        {

            GetComponent<AudioSource>().Stop();

            GetComponent<AudioSource>().clip = BGM3;
            GetComponent<AudioSource>().Play();
        }
        if (num == 5)
        {

            GetComponent<AudioSource>().Stop();

            GetComponent<AudioSource>().clip = BGM4;
            GetComponent<AudioSource>().Play();
        }
    }
}
