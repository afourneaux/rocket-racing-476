using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutForMenu : MonoBehaviour
{
    public GameObject TutorialPanel;

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

        if (!active)
        {
            TutorialPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
