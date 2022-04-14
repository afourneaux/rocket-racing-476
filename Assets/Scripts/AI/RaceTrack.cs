using System.Collections.Generic;
using UnityEngine;

// Contains all the data necessary for the AI to race properly
// Only one RaceTrack component should be in a scene at the time

// The component also requires the RacerManager component to make sure the 
// racers have that component in the scene to properly work
[RequireComponent(typeof(RacerManager))]
public class RaceTrack : MonoBehaviour
{
    private static RaceTrack CurrRaceTrack; // The current race track to be raced uppon

    [SerializeField]
    private List<Transform> trackPositions; // Used to set the positions of the track in the editor

    private List<Vector3> positions = new List<Vector3>(); // Used for the actual AI

    private void Awake()
    {
        //Track position will get all the track points under "Race Track" every time
        trackPositions.Clear();
        foreach (Transform t in GameObject.Find("Race Track").GetComponentInChildren<Transform>())
            trackPositions.Add(t);

        CurrRaceTrack = this;    
        ConvertTransformToVectorList();
    }

    // Used by the AI to retrieve the path to follow
    public static List<Vector3> GetPathPositions()
    {
        return CurrRaceTrack.positions;
    }

    private void ConvertTransformToVectorList()
    {
        if (trackPositions.Count < 3)
        {
            Debug.LogError("The race track needs at least 3 positions");
            return;
        }

        foreach(Transform t in trackPositions)
        {
            positions.Add(t.position);
        }
    }
}
