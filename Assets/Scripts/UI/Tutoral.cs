using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutoral : MonoBehaviour
{
    public GameObject TuturalPanel;
    private bool active = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            active = !active;

        }
        if (active)
        {
            TuturalPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            TuturalPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
