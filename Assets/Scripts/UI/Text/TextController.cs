using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    public int velocityMultiplier = 10;

    public Text timerText;
    public Text positionText;
    public Text speedText;

    private int playerIndex;

    public enum TextTypes{ timer, position, speed};

    // Start is called before the first frame update
    void Start()
    {
        ResetTexts();
        playerIndex = GetPlayerIndex();
    }

    public void Update()
    {
        SetText(TextTypes.position, RacerManager.GetPositionTracker(playerIndex).GetPosition().ToString());
        SetText(TextTypes.speed, ((int)(RacerManager.GetRigidbody(playerIndex).velocity.magnitude * velocityMultiplier)).ToString());
        SetText(TextTypes.timer, RacerManager.GetTimeElapsed().ToString("F2"));
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
            case TextTypes.position: 
                switch (text) 
                {
                    case "1": text += "st"; break;
                    case "2": text += "nd"; break;
                    case "3": text += "rd"; break;
                    default: text += "th"; break;
                }
                positionText.text = text;
                break;
            case TextTypes.speed: speedText.text = text; break;
        }
    }

    private int GetPlayerIndex() 
    {
        var racers = RacerManager.GetRacers();

        for (int i = 0; i < racers.Count; i++)
        {
            if (racers[i].gameObject.GetComponent<PlayerRacer>() != null) 
            {
                return i;
            }
        }

        return racers.Count -1;
    }
}
