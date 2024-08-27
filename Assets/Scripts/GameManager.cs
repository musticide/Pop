using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int lives = 3;
    private int score = 0;

    public UIManager uiManager;

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

