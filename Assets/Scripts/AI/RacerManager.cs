using System.Collections.Generic;
using UnityEngine;

// Singleton responsible for holding information about the racers in the current race
public class RacerManager : MonoBehaviour
{
    private static RacerManager Instance;

    private List<Rigidbody> racerRigidbodies = new List<Rigidbody>();
    private List<PositionTracker> positionTrackers = new List<PositionTracker>();

    private void Awake()
    {
        Instance = this;
        
        // Clear previous information
        racerRigidbodies.Clear(); 
        positionTrackers.Clear();
    }

    private void Update()
    {
        UpdatePositions();
    }

    public static void AddRacer(Rigidbody rb) { Instance.racerRigidbodies.Add(rb); }

    public static void AddTracker(PositionTracker tracker) { Instance.positionTrackers.Add(tracker); }

    public static List<Rigidbody> GetRacers() { return Instance.racerRigidbodies; }

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
            positionList[i].SetPosition(positionList.Count - i);
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
}
