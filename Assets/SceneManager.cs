using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] GameObject gameScene;
    public void LoadGamePrefab()
    {
        // Instantiate(gameScene);
        gameScene.SetActive(true);
    }

}
