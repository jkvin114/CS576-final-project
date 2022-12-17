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
    public Transform food01;
    public Transform food02;
    public Transform food03;
    public Transform food04;
    public Transform food05;
    public List<Transform> foodPrefabList = new List<Transform>();
    public List<GameObject> foodPrefabs;

    public GameObject Apple;
    public GameObject Banana;
    public GameObject Candy;
    public GameObject Garlic;
    public GameObject Hamburger;
    public GameObject Mushroom;
    public GameObject Lemon;
    public GameObject Pumpkin;
    public GameObject Waffle;
    public GameObject Watermelon;

    public TMP_Text currentScoreText;
    public TMP_Text totalScoreText;
    public TMP_Text runCountText;
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;
    public GameObject Heart4;
    public GameObject Heart5;
    public int currentScore;
    public int totalScore;
    public int lifePoint;
    public int runCount;
    public List<int> foodIntList = new List<int>(new int[5]);

    public int currPos;
    public GameObject Q1;
    public GameObject Q2;
    public GameObject Q3;
    public GameObject Q4;
    public GameObject Q5;
    public GameObject C1;
    public GameObject C2;
    public GameObject C3;
    public GameObject C4;
    public GameObject C5;

    public List<GameObject> QList = new List<GameObject>();
    public List<GameObject> CList = new List<GameObject>();

    public Level level;

    public GameObject player;
    bool alive = true;
    // Start is called before the first frame update
    void Start()
    {
        BGM.BGM_instance.GetComponent<AudioSource>().Play();

        foodPrefabList.Add(food01);
        foodPrefabList.Add(food02);
        foodPrefabList.Add(food03);
        foodPrefabList.Add(food04);
        foodPrefabList.Add(food05);

        foodPrefabs = new List<GameObject>() {Apple,Banana,Candy,Garlic,Hamburger,Mushroom,Lemon,Pumpkin,Waffle,Watermelon };
        currPos = 0;
        QList.Add(Q1);
        QList.Add(Q2);
        QList.Add(Q3);
        QList.Add(Q4);
        QList.Add(Q5);
        CList.Add(C1);
        CList.Add(C2);
        CList.Add(C3);
        CList.Add(C4);
        CList.Add(C5);

        runCount = 0;
        Time.timeScale = 0;
        startTime = 120f;
        startTimeMemory = 7f;
        currTime = startTime;
        currTimeMemory = startTimeMemory;

        currentScore = 0;
        totalScore = 0;
        lifePoint = 5;

        GenerateFoodList();

        Directory.CreateDirectory(Application.streamingAssetsPath + "/InputLogs");
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        currTime -= 1 * Time.deltaTime;
        currTimeMemory -= 1 * Time.deltaTime;
        counterText.text = currTime.ToString("0");
        counterTextMemory.text = currTimeMemory.ToString("0");
        runCountText.text = runCount.ToString("0");

        if (currTime > 0)
        {
            currentScoreText.text = currentScore.ToString();
            totalScoreText.text = totalScore.ToString();
            
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
                alive = false;
                Heart1.SetActive(false);

                totalScore += currentScore;
                player.GetComponent<Fox>().death();
                StartCoroutine(onDeath());
            }
        }
        else if (currTime <= 0) {
            TimeOver();
        }
    }
    IEnumerator onDeath()
    {
        yield return new WaitForSeconds(3);
        PlayerPrefs.SetString("score", totalScore.ToString());
        BGM.BGM_instance.GetComponent<AudioSource>().Stop();
        SFXManager.sfx_instance.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene("GameOver");
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Score);

        yield return null;
    }

    public void HitsNormalGem() {
        currentScore += 51;
    }

    public void HitsSpecialGem() {
        currentScore += 444;
    }

    public void HitsCorrectFood() {
        currentScore += 5553;
        QList[currPos].SetActive(false);
        CList[currPos].SetActive(true);
        currPos += 1;
        level.onGetRightFood();
    }
    
    public void HitsWrongFood() {
        currentScore -= 2929;
    }

    public void CompletesList() {
        currentScore += 33337;
        if(lifePoint < 5) {
            lifePoint += 1;
        }
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.FoodComplete);
        runCount += 1;

        currTime = startTime;
        currTimeMemory = startTimeMemory;
        foodList.SetActive(true);
        tracker.SetActive(false);
        counterTextMemory.enabled = true;
        disappearText.enabled = true;
        totalScore += currentScore;
        currentScore = 0;
        currPos = 0;
        Q1.SetActive(true);
        Q2.SetActive(true);
        Q3.SetActive(true);
        Q4.SetActive(true);
        Q5.SetActive(true);
        C1.SetActive(false);
        C2.SetActive(false);
        C3.SetActive(false);
        C4.SetActive(false);
        C5.SetActive(false);

        DestroyFoodList();
        GenerateFoodList();

            BGM.BGM_instance.GetComponent<BGM>().onRunCountUp(runCount);
        
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
        currPos = 0;
        Q1.SetActive(true);
        Q2.SetActive(true);
        Q3.SetActive(true);
        Q4.SetActive(true);
        Q5.SetActive(true);
        C1.SetActive(false);
        C2.SetActive(false);
        C3.SetActive(false);
        C4.SetActive(false);
        C5.SetActive(false);

        DestroyFoodList();
        GenerateFoodList();
    }

    public void HitsObstacle() {
        lifePoint -= 1;
    }

    public void GenerateFoodList() {
        int newNumber;
        int i = 0;
        player.GetComponent<Fox>().setInvulnerable((int)startTimeMemory);
        while(i < 5) {
            newNumber = Random.Range(1,11);
            if(!foodIntList.Contains(newNumber)) {
                foodIntList[i]=(newNumber);
                i++;
            }
        }
        for(int j = 0; j < 5; j++) {
            SpawnFood(j, foodIntList[j]);
        }
        level.onSpawnFood(foodIntList);
    }

    public void DestroyFoodList() {
        for(int i = 0; i < 5; i++) {
            Destroy(foodPrefabList[i].GetChild(0).gameObject);
        }
    }

    public void SpawnFood(int pos, int numFood) {
        if(numFood == 1) {
            GameObject newFood = Instantiate(Apple, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 2) {
            GameObject newFood = Instantiate(Banana, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 3) {
            GameObject newFood = Instantiate(Candy, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 4) {
            GameObject newFood = Instantiate(Garlic, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 5) {
            GameObject newFood = Instantiate(Hamburger, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 6) {
            GameObject newFood = Instantiate(Mushroom, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 7) {
            GameObject newFood = Instantiate(Lemon, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 8) {
            GameObject newFood = Instantiate(Pumpkin, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 9) {
            GameObject newFood = Instantiate(Waffle, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
        else if(numFood == 10) {
            GameObject newFood = Instantiate(Watermelon, foodPrefabList[pos]) as GameObject;
            newFood.transform.localScale = new Vector3(250, 250, 1);
            newFood.gameObject.layer = 5;
        }
    }
}
