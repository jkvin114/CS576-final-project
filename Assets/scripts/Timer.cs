using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Timer : MonoBehaviour
{

    public float currTime;
    public float startTime;
    public float currTimeMemory;
    public float startTimeMemory;
    public Text counterText;
    public Text counterTextMemory;
    public Text disappearText;
    public GameObject foodList;
    public GameObject tracker;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        startTime = 50f;
        startTimeMemory = 3f;
        currTime = startTime;
        currTimeMemory = startTimeMemory;
    }

    // Update is called once per frame
    void Update()
    {
        currTime -= 1 * Time.deltaTime;
        currTimeMemory -= 1 * Time.deltaTime;
        counterText.text = currTime.ToString("0");
        counterTextMemory.text = currTimeMemory.ToString("0");

        if(currTime <= 0) {
            SceneManager.LoadScene("GameOver");
        }
        if(currTimeMemory <= 0) {
            foodList.SetActive(false);
            tracker.SetActive(true);
            counterTextMemory.enabled = false;
            disappearText.enabled = false;
        }
    }
}
