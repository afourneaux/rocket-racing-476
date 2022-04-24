using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float timeRate = 1.0f;
    [SerializeField]
    private GameObject UIDisplay;
    private Text textDisplay;

    // Attach this script to an object to set the timescale of the scene
    void Start()
    {
        Time.timeScale = timeRate;
        if (UIDisplay != null) {
            UIDisplay.GetComponent<Text>().text = "Time scale: " + timeRate.ToString("0.0#X");
        }
    }
}
