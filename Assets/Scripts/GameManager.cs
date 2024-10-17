using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int lives = 3;
    private int score = 0;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private SceneManager sceneManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //init UI
        uiManager.SetLifeText(lives);
        uiManager.SetScoreText(score);

        uiManager.startButton.onClick.AddListener(StartGame);

    }

    private void StartGame()
    {
        Debug.Log("Game Started!");
        uiManager.ShowGameUI();
        sceneManager.LoadGamePrefab();
    }

    public void LoseLife()
    {
        lives--;
        uiManager.SetLifeText(lives);
        if (lives <= 0)
        {
            EndGame();
        }
    }
    public void GainLife()
    {
        lives++;
        uiManager.SetLifeText(lives);
    }

    public void AddScore()
    {
        score++;
        uiManager.SetScoreText(score);
    }

    public int GetLife()
    {
        return lives;
    }
    public int GetScore()
    {
        return score;
    }

    private void EndGame()
    {
        // Handle game over
    }
}

