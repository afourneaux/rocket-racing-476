using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutoral : MonoBehaviour
{
    [SerializeField]
    private GameObject TutorialPanel;
    [SerializeField]
    private PauseMenu pause;
    private bool active = false;

    private float prevTimeScale;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !pause.getGameIsPaused())
        {
            ToggleTutorialMenu();
        }

        if (active && pause.getGameIsPaused())
        {
            active = false;
            TutorialPanel.SetActive(false);
        }
    }

    private void ToggleTutorialMenu()
    {
        if (active)
        {
            TutorialPanel.SetActive(false);
            active = false;
            Time.timeScale = prevTimeScale;
            return;
        }

        active = true;
        TutorialPanel.SetActive(true);

        if (Time.timeScale != 0)
        {
            prevTimeScale = Time.timeScale;
        }
        else
        {
            prevTimeScale = 1.0f;
        }
        Time.timeScale = 0;
    }
}
