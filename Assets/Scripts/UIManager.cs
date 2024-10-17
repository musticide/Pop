using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] GameObject gameHUD;
    [SerializeField] GameObject mainMenu;
    List<GameObject> uiCanvases = new List<GameObject>();

    [Header("Main Menu")]
    public Button startButton;
    [Header("Game HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lifeText;

    private void Awake()
    {
        AddCanvas(mainMenu);
        AddCanvas(gameHUD);

        uiCanvases.ForEach(x => { x.SetActive(false); });

        mainMenu.SetActive(true);
    }

    private void AddCanvas(GameObject canvas)
    {
        if (canvas) uiCanvases.Add(canvas);
    }

    private void HideAllCanvases()
    {
        uiCanvases.ForEach(x => { x.SetActive(false); });
    }

    public void ShowGameUI()
    {
        uiCanvases.ForEach(x => { x.SetActive(false); });
        gameHUD.SetActive(true);
    }

    public void ShowMainMenu()
    {
        HideAllCanvases();
        mainMenu.SetActive(true);
    }

    public void SetLifeText(int lives)
    {
        lifeText.text = "Life: " + lives.ToString();
    }

    public void SetScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

}

