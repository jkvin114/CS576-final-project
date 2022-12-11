using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;

    public void OpenMenu() {
        Debug.Log("click");
        menu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseMenu() {
        menu.gameObject.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("click");
    }
}
