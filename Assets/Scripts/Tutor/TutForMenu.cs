using UnityEngine;

public class TutForMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject TutorialPanel;

    private bool active = false;
    private float prevTimeScale;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleTutorialMenu();
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
