using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Timer : MonoBehaviour
{

    public float currTime;
    public float startTime;
    public float currTimeMemory;
    public float startTimeMemory;
    public Text counterText;
    public Text counterTextMemory;


    // Start is called before the first frame update
    void Start()
    {
        startTime = 15f;
        startTimeMemory = 5f;
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
    }
}
