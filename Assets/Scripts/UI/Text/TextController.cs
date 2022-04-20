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

    private Rigidbody playerRB;
    private PositionTracker positionTracker;

    public enum TextTypes{ timer, position, speed};

    public void StartRace() 
    {
        ResetTexts();
        ResetPlayerReferences();
    }

    public void Update()
    {
        if (RacerManager.GetTimeElapsed() < 0) 
        {
            return;
        }
   
        if (playerRB != null && positionTracker != null)
        {
            SetText(TextTypes.position, positionTracker.GetPosition().ToString());
            SetText(TextTypes.speed, ((int)(playerRB.velocity.magnitude * velocityMultiplier)).ToString());
            SetText(TextTypes.timer, RacerManager.GetTimeElapsed().ToString("F2"));
        }
    }

    public void ResetTexts()
    {
        timerText.text = "00:00:00";
        positionText.text = "1st";
        speedText.text = "0";
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

    private void ResetPlayerReferences() 
    {
        int playerIndex = GetPlayerIndex();
        positionTracker = RacerManager.GetPositionTracker(playerIndex);
        playerRB = RacerManager.GetRigidbody(playerIndex);
    }
}
