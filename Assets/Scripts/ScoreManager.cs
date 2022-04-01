using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//singleton class which is responsible for holding the score data of each racer and displaying it at the end of the race
public class ScoreManager : MonoBehaviour
{
    private static ScoreManager Instance;

    [SerializeField]
    private GameObject endScreenObj;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    public int MaxTimeScore;
    [SerializeField]
    public int MinTimeScore;
    private List<ScoreData> racerScores = new List<ScoreData>();
    private float FirstRacerFinishedTime = 0.0f;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        racerScores.Clear();
        endScreenObj.SetActive(false);
    }

    public static void FinishRace()
    {
        Instance.endScreenObj.SetActive(true);
        string scoreStr = "";
        Debug.Log("num scores: " + Instance.racerScores.Count);
        for(int i = 0; i < Instance.racerScores.Count; i++)
        {
            ScoreData currScore = Instance.racerScores[i];
            scoreStr += "#" + (i + 1) + "." + currScore.GetName() + "   Accuracy Score: " 
                + currScore.GetAccuracyScore() + "   Time Score: " + currScore.GetTimeScore() 
                + "   Total Score: " + currScore.GetTotalScore() + "\n";
        }
        Instance.scoreText.text = scoreStr;
    }

    // inserts the provided score in the list so the list is already sorted from biggest 
    //score (index 0) to smallest score (index Count-1)
    public static void AddScore(ScoreData score)
    {
        for (int i = 0; i < Instance.racerScores.Count; i++)
        {
            if (score.GetTotalScore() > Instance.racerScores[i].GetTotalScore())
            {
                Instance.racerScores.Insert(i, score);
                return;
            }
        }
        Instance.racerScores.Add(score);
    }

    public static List<ScoreData> GetScoreData() { return Instance.racerScores; }
    public static int GetMinScore() { return Instance.MinTimeScore; }
    public static int GetMaxScore() { return Instance.MaxTimeScore; }

    public static void saveFirstRacerFinishedTime() { Instance.FirstRacerFinishedTime = Time.time; }
    public static float GetFirstRacerFinishedTime() { return Instance.FirstRacerFinishedTime; }
}
