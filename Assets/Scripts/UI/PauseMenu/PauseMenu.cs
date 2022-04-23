using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    private float prevTimeScale;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
       
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = prevTimeScale;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (Time.timeScale != 0)
        {
            prevTimeScale = Time.timeScale;
        }
        else
        {
            prevTimeScale = 1.0f;
        }
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
        Destroy(AudioManager.Instance);
    }

    public bool getGameIsPaused()
    {
        return GameIsPaused;
    }
}
