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
    private bool displayTrack = false; // For debugging only

    [SerializeField]
    private List<Transform> trackPositions; // Used to set the positions of the track in the editor

    private List<Vector3> positions = new List<Vector3>(); // Used for the actual AI

    // Used to display the track. Only used for debugging
    private LineRenderer trackRenderer = null;

    private void Awake()
    {
        //Track position will get all the track points under "Race Track" every time
        trackPositions.Clear();
        foreach (Transform t in GameObject.Find("Race Track").GetComponentInChildren<Transform>())
            trackPositions.Add(t);

        CurrRaceTrack = this;    
        ConvertTransformToVectorList();
    }

    private void Update()
    {
        if (!displayTrack)
        {
            // Do not create the line renderer component if we are not debugging
            if (trackRenderer != null && trackRenderer.positionCount != 2)
            {
                trackRenderer.positionCount = 2;
                trackRenderer.SetPositions(new Vector3[0]);
            }
            return;
        }

        LineRenderer renderer = GetLineRenderer();
        if (renderer.positionCount == 2)
        {
            renderer.positionCount = positions.Count;
            renderer.SetPositions(positions.ToArray());
        }
        
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

    private LineRenderer GetLineRenderer()
    {
        if (trackRenderer != null) 
        {
            return trackRenderer;
        }

        trackRenderer = GetComponent<LineRenderer>();
        if (trackRenderer == null)
        {
            trackRenderer = gameObject.AddComponent<LineRenderer>();
        }
        trackRenderer.useWorldSpace = true;
        return trackRenderer;
    }
}
