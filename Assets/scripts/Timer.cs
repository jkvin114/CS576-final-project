using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System;

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

    public TMP_Text currentScoreText;
    public int currentScore;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        startTime = 50f;
        startTimeMemory = 3f;
        currTime = startTime;
        currTimeMemory = startTimeMemory;

        currentScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currTime -= 1 * Time.deltaTime;
        currTimeMemory -= 1 * Time.deltaTime;
        counterText.text = currTime.ToString("0");
        counterTextMemory.text = currTimeMemory.ToString("0");

        currentScoreText.text = currentScore.ToString();

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

    public void HitsNormalGem() {
        currentScore += 5;
    }

    public void HitsSpecialGem() {
        currentScore += 20;
    }

    public void HitsCorrectFood() {
        currentScore += 50;
    }

    public void HitsWrongFood() {
        currentScore -= 10;
    }

    public void CompletesList() {
        currentScore += 100;
    }

    public void TimeOver() {
        currentScore -= 100;
    }
}
