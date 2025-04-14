using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWin;
    public Slider progressSlider;
    public int MaxProgress = 5;

    public static event Action OnRestart;
    public static event Action OnPlayerWin;
    
    int progressAmount;
    
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerHealth.OnPlayerDeath += GameOverScreen;
        OnPlayerWin += WinScreen;
        progressAmount = 0;
        progressSlider.value = progressAmount;
        PlayerItemController.OnBreadCollect += IncreaseProgress;

    }

    void IncreaseProgress(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        progressSlider.maxValue = MaxProgress;
        Debug.Log("Progress: " + progressAmount);
        if (progressAmount >= MaxProgress)
        {
            OnPlayerWin.Invoke();
            Debug.Log("You win!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameOverScreen()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    void WinScreen()
    {
        gameWin.SetActive(true);
        Time.timeScale = 0;
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void Restart()
    {
        gameOverUI.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        OnRestart.Invoke();
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
