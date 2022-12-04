using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;

    public void OpenMenu() {
        menu.gameObject.SetActive(true);
    }

    public void CloseMenu() {
        menu.gameObject.SetActive(false);
    }
}
