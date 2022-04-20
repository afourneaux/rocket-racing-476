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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            active = !active;

        }
        if (active)
        {
            TutorialPanel.SetActive(true);
            Time.timeScale = 0;
        }
        if (!active && pause.getGameIsPaused())
            TutorialPanel.SetActive(false);

        if (!active && !pause.getGameIsPaused())
        {
            TutorialPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
