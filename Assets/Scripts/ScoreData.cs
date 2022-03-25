using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreData : MonoBehaviour
{
    private int accuracyScore = 0;
    private int timeScore = 0;

    public int GetTotalScore() { return accuracyScore + timeScore; }

    public int GetAccuracyScore() { return accuracyScore; }
    public void SetAccuracyScore(int accuracy)
    {
        accuracyScore = accuracy;
    }

    public int GetTimeScore() { return timeScore; }
    public void SetTimeScore(int time)
    {
        timeScore = time;
    }
}
