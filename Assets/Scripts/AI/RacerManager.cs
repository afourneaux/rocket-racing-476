using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton responsible for holding information about the racers in the current race
public class RacerManager : MonoBehaviour
{
    private static RacerManager Instance;

    private static List<Rigidbody> racerRigidbodies = new List<Rigidbody>();
    private static List<PositionTracker> positionTrackers = new List<PositionTracker>();
    private int numFinishedRacers;
    private bool firstHit = true;

    private static float timeElapsed;

    private static bool raceFinished = false;

    private static TextController textController;


    private void Awake()
    {
        Instance = this;
        
        // Clear previous information
        racerRigidbodies.Clear(); 
        positionTrackers.Clear();
    }

    private void Start()
    {
        firstHit = true;
        numFinishedRacers = 0;
        textController = gameObject.GetComponent<TextController>();
        StartRace();
        raceFinished = false;
        timeElapsed = -CountdownController.Instance.getCountdownTime();
    }

    private void Update()
    {
        UpdatePositions();
        if (!raceFinished)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public static void StartRace() 
    {
        timeElapsed = -CountdownController.Instance.getCountdownTime();
        textController.StartRace();
    }

    public static void FinishRace(PositionTracker tracker)
    {
        Instance.numFinishedRacers++;
        positionTrackers.Remove(tracker);
        raceFinished = tracker.GetComponent<PlayerRacer>() != null || raceFinished;

        if (Instance.firstHit)
        {
            Instance.firstHit = false;
            ScoreManager.saveFirstRacerFinishedTime();
        }
    }

    public static void AddRacer(Rigidbody rb) { racerRigidbodies.Add(rb); }
    
    public static void RemoveRacer(Rigidbody rb) 
    {
       if (Instance == null)
       {
            //if we just unloaded the scene just exit
            return;
       }

       racerRigidbodies.Remove(rb); 
       if (racerRigidbodies.Count == 0)
       {
           ScoreManager.FinishRace();
       }
    }

    public static void AddTracker(PositionTracker tracker) { positionTrackers.Add(tracker); }

    public static List<Rigidbody> GetRacers() { return racerRigidbodies; }

    private void UpdatePositions()
    {
        List<PositionTracker> positionList = new List<PositionTracker>();
        foreach (PositionTracker tracker in positionTrackers)
        {
            tracker.UpdateTrackIndex();
            AddRacerToPositionList(positionList, tracker);
        }

        for (int i = 0; i < positionList.Count; i++)
        {
            positionList[i].SetPosition(positionList.Count - i + numFinishedRacers);
        }
    }

    private void AddRacerToPositionList(List<PositionTracker> positionList, PositionTracker racerToAdd)
    {
        for (int i = 0; i < positionList.Count; i++)
        {
            if (racerToAdd.GetTrackIndex() == positionList[i].GetTrackIndex())
            {
                List<Vector3> pathNodes = RaceTrack.GetPathPositions();
                int currNodeIndex = racerToAdd.GetTrackIndex();
                Vector3 consideredPathNode = currNodeIndex == pathNodes.Count - 1 
                    ? pathNodes[currNodeIndex] : pathNodes[currNodeIndex + 1];

                if (Vector3.Distance(racerToAdd.transform.position, consideredPathNode) 
                    > Vector3.Distance(positionList[i].transform.position, consideredPathNode))
                {
                    positionList.Insert(i, racerToAdd);
                    return;
                }
            }
            else if (racerToAdd.GetTrackIndex() < positionList[i].GetTrackIndex())
            {
                positionList.Insert(i, racerToAdd);
                return;
            }
        }
        positionList.Add(racerToAdd);
    }

    public static Rigidbody GetRigidbody(int index) { return racerRigidbodies[index]; }

    public static PositionTracker GetPositionTracker(int index) { return positionTrackers[index]; }

    public static float GetTimeElapsed() 
    {
        return timeElapsed;
    }

}
