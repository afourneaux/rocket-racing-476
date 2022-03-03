using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    public Text timerText;
    public Text positionText;
    public Text speedText;

    public enum TextTypes{ timer, position, speed};

    // Start is called before the first frame update
    void Start()
    {
        ResetTexts();
    }

    public void ResetTexts()
    {
        timerText.text = "00:00:00";
        positionText.text = "1st";
        speedText.text = "0km";
    }

    public void SetText(TextTypes type, string text) 
    {
        switch (type) 
        {
            case TextTypes.timer: timerText.text = text; break;
            case TextTypes.position: positionText.text = text; break;
            case TextTypes.speed: speedText.text = text; break;
        }
    }
}
