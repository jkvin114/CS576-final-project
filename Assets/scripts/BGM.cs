using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM BGM_instance;

    private void Awake() {
        if(BGM_instance != null && BGM_instance != this) {
            Destroy(this.gameObject);
            return;
        }

        BGM_instance = this;
        DontDestroyOnLoad(this);
    }
}
