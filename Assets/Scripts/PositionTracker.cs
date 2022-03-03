using UnityEngine;

// Keeps track of the position of the racer on which this tracker is.
public class PositionTracker : MonoBehaviour
{
    private int currPathIndex = 0;
    private int currPosition = -1;

    private void Start()
    {
        RacerManager.AddTracker(this);
    }

    // Temporary, for demonstration purposes only
    private void Update()
    {
        if  (currPosition == 1)
        {
            Debug.Log(gameObject.name + " is in first place");
        }
    }

    // Returns the path index inside the node of the path
    public int GetTrackIndex()
    {
        return currPathIndex;
    }

    public int GetPosition() { return currPosition; }
    public void SetPosition(int position) { currPosition = position; }

    public void UpdateTrackIndex()
    {
        currPathIndex = BasicAI.GetNextPathIndex(RaceTrack.GetPathPositions(), transform.position, currPathIndex);
    }
}
