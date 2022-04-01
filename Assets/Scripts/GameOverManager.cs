using System.Collections.Generic;
using UnityEngine;


public class GameOverManager : MonoBehaviour
{
    private static GameOverManager Instance;

    List<Rigidbody> finishedRacers;
    public GameObject gameOverScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        finishedRacers = new List<Rigidbody>();
    }


    public static void AddFinishedRacer(Rigidbody racer)
    {
        if (!Instance.finishedRacers.Contains(racer))
        {
            Instance.finishedRacers.Add(racer);
            if (Instance.finishedRacers.Count == RacerManager.GetRacers().Count)
            {
                Instance.gameOverScreen.SetActive(true);
            }
        }
    }
}
