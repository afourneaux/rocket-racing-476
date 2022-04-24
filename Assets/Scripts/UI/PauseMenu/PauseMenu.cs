using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static bool GameIsPaused;

    [SerializeField]
    private GameObject pauseMenuUI;

    private float prevTimeScale;
    private void Start()
    {
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

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

    public static bool getGameIsPaused()
    {
        return GameIsPaused;
    }
}
