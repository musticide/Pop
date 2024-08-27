using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lifeText;
    // public GameManager gameManager;

    // void Start()
    // {
    //     lifeText.text = "Life: " + gameManager.GetLife().ToString();
    //     scoreText.text = "Score: " + gameManager.GetScore().ToString();
    // }

    public void SetLifeText(int lives)
    {
        lifeText.text = "Life: " + lives.ToString();
    }

    public void SetScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

}

