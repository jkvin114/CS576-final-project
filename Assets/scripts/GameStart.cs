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
        string path = @"C:\Github\CS576-final-project\Assets\StreamingAssets\InputLogs\score.txt";
        string displayScore = File.ReadAllText(path);
        scoreText.text = displayScore;
    }

    public void LoadGame() {
        SceneManager.LoadScene("MainGame");
    }
}
