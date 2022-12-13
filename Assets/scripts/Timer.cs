using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;
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
    public TMP_Text totalScoreText;
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;
    public GameObject Heart4;
    public GameObject Heart5;
    public int currentScore;
    public int totalScore;
    public int lifePoint;
    public List<int> foodIntList = new List<int>(new int[5]);


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        startTime = 5f;
        startTimeMemory = 3f;
        currTime = startTime;
        currTimeMemory = startTimeMemory;

        currentScore = 0;
        totalScore = 0;
        lifePoint = 5;

        int newNumber;
        int i = 0;

        Debug.Log("test start");
        while(i < 5) {
            newNumber = Random.Range(0,9);
            Debug.Log(newNumber);
            if(!foodIntList.Contains(newNumber)) {
                foodIntList[i] = newNumber;
                i++;
            }
        }
        Debug.Log("test end");
        Debug.Log("test start");
        for(int j = 0; j < 5; j++) {
           //Debug.Log(foodIntList[j]);
        }
        Debug.Log("test end");
    }

    // Update is called once per frame
    void Update()
    {
        currTime -= 1 * Time.deltaTime;
        currTimeMemory -= 1 * Time.deltaTime;
        counterText.text = currTime.ToString("0");
        counterTextMemory.text = currTimeMemory.ToString("0");

        if (currTime <= 0)
        {

            currentScoreText.text = currentScore.ToString();
            totalScoreText.text = totalScore.ToString();

            if (currTime <= 0)
            {
               // TimeOver();
            }
            if (currTimeMemory <= 0)
            {
                foodList.SetActive(false);
                tracker.SetActive(true);
                counterTextMemory.enabled = false;
                disappearText.enabled = false;
            }

            if (lifePoint == 5)
            {
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(true);
                Heart4.SetActive(true);
                Heart5.SetActive(true);
            }
            else if (lifePoint == 4)
            {
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(true);
                Heart4.SetActive(true);
                Heart5.SetActive(false);
            }
            else if (lifePoint == 3)
            {
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(true);
                Heart4.SetActive(false);
                Heart5.SetActive(false);
            }
            else if (lifePoint == 2)
            {
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(false);
                Heart4.SetActive(false);
                Heart5.SetActive(false);
            }
            else if (lifePoint == 1)
            {
                Heart1.SetActive(true);
                Heart2.SetActive(false);
                Heart3.SetActive(false);
                Heart4.SetActive(false);
                Heart5.SetActive(false);
            }
            else if (lifePoint > 5)
            {
                lifePoint = 5;
                Heart1.SetActive(true);
                Heart2.SetActive(true);
                Heart3.SetActive(true);
                Heart4.SetActive(true);
                Heart5.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene("GameOver");
            }
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
        if(lifePoint < 5) {
            lifePoint += 1;
        }
        totalScore += currentScore;
        currentScore = 0;
    }

    public void TimeOver() {
        currentScore -= 100;
        lifePoint -= 1;
       // Debug.Log("Lost 1 health!!!!!!");
        currTime = startTime;
        currTimeMemory = startTimeMemory;
        foodList.SetActive(true);
        tracker.SetActive(false);
        counterTextMemory.enabled = true;
        disappearText.enabled = true;
        totalScore += currentScore;
        currentScore = 0;
    }

    public void HitsObstacle() {
        lifePoint -= 1;
    }
}
