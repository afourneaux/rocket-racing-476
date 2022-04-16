using UnityEngine;

// Keeps track of the position of the racer on which this tracker is.
[RequireComponent(typeof(ScoreData))]
public class PositionTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionEffectPrefab;

    private int currPathIndex = 0;
    private int currPosition = -1;
    private bool hasFinishedRace;
    private ScoreData score;
    private void Start()
    {
        score = new ScoreData(gameObject.name);
        hasFinishedRace = false;
        RacerManager.AddTracker(this);

    }

    // Returns the path index inside the node of the path
    public int GetTrackIndex()
    {
        return currPathIndex;
    }

    public int GetPosition() { return currPosition; }
    
    public void SetPosition(int position)
    {
        // Only update the position if the racer has not finished the race
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
            if (hasFinishedRace)
            {
                return;
            }

            Bullseye bullseyeObj = other.GetComponent<Bullseye>();

            // Fetch collision Point
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                // Make sure we collided with the circular part of the collider 
                // and not the corners before finishing the race
                if (bullseyeObj.HasCollidedWith(hitInfo.point))
                {
                    FinishRace(bullseyeObj, hitInfo.point);
                }
            }
        }
    }

    private void FinishRace(Bullseye bullseyeObj, Vector3 bullseyeHitPoint)
    {
        score.SetAccuracyScore(bullseyeObj.GetScore(bullseyeHitPoint));
        RacerManager.FinishRace(this);
       
        // Compute time score here
        score.SetTimeScore((int)Mathf.Lerp((float)ScoreManager.GetMinScore(), 
            (float)ScoreManager.GetMaxScore(), ScoreManager.GetFirstRacerFinishedTime() / Time.time));
        
        hasFinishedRace = true;
        ScoreManager.AddScore(score);
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        FindObjectOfType<AudioManager>().Play("explosion");
        Destroy(gameObject);
    }

}
