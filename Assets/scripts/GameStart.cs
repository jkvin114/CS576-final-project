using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class GameStart : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start() {
       // string path = @"\Assets\StreamingAssets\InputLogs\score.txt";
  //      string displayScore = File.ReadAllText(path);
        

        scoreText.text = PlayerPrefs.GetString("score"); ;
    }

    public void LoadGame() {
        SceneManager.LoadScene("MainGame");
    }
}
