using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject healthBar;
    public GameObject gameOverPanel;

    [Header("UI Elements")]
    public GameObject pauseMenu;
    public Slider bossHealthBar;

    private void Awake()
    {
        if(instance == null)
            instance =this;
        else
            Destroy(gameObject);
        
        
    }

    public void UpdateHealth(float currentHealth)
    {
        switch(currentHealth)
        {
            case 3:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(true);
                healthBar.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 2:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(true);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 1:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(false);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 0:
                healthBar.transform.GetChild(0).gameObject.SetActive(false);
                healthBar.transform.GetChild(1).gameObject.SetActive(false);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;

        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale =0;
    }

    public void ResumGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void SetBossHealth(float health)
    {
        bossHealthBar.maxValue = health;
    }

    public void UpdateBossHealth(float health)
    {
        bossHealthBar.value = health;
    }

    public void GameOverUI(bool playerDead)
    {
        if(gameOverPanel)
            gameOverPanel.SetActive(playerDead);
    }
}
