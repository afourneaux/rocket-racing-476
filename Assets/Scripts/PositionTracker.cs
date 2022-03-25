using UnityEngine;

// Keeps track of the position of the racer on which this tracker is.
[RequireComponent(typeof(ScoreData))]
public class PositionTracker : MonoBehaviour
{
    private int currPathIndex = 0;
    private int currPosition = -1;

    private bool hasFinishedRace;

    private void Start()
    {
        hasFinishedRace = false;
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
    public void SetPosition(int position)
    {
        //only update the position if the racer has not finished the race
        if (!hasFinishedRace)
        {
            currPosition = position;
        }
    }

    public void UpdateTrackIndex()
    {
        currPathIndex = BasicAI.GetNextPathIndex(RaceTrack.GetPathPositions(), transform.position, currPathIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullseye"))
        {
            Bullseye bullseyeObj = other.GetComponent<Bullseye>();

            //fetch collision Point
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                //make sure we collided with the circular part of the collider and not the corners
                if (bullseyeObj.HasCollidedWith(hitInfo.point))
                {
                    ScoreData score = GetComponent<ScoreData>();
                    score.SetAccuracyScore(bullseyeObj.GetScore(hitInfo.point));
                    hasFinishedRace = true;

                }
            }
        }
    }
}
