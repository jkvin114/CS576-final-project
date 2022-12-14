using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject tutorial;

    public void OpenMenu() {
        menu.gameObject.SetActive(true);
        tutorial.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseMenu() {
        menu.gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
