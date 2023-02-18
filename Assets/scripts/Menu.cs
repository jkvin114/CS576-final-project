using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject tutorial;
    bool paused=false;
    public void OpenMenu() {
        menu.gameObject.SetActive(true);
        tutorial.gameObject.SetActive(true);
        Time.timeScale = 0;
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Click);
        paused= true;
    }

    public void CloseMenu() {
        paused= false;
        menu.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);
        Time.timeScale = 1;
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Click);
    }
    private void Update()
    {
        if(paused)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
        }
    }
}
