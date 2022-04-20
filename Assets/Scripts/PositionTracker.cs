using System.Collections.Generic;
using UnityEngine;

// Keeps track of the position of the racer on which this tracker is.
[RequireComponent(typeof(ScoreData), typeof(Rigidbody))]
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
        List<Vector3> path = RaceTrack.GetPathPositions();
        int closestIndex = 0;
        float smallestDistance = Vector3.Distance(path[0], transform.position);

        for (int i = 1; i < path.Count; i++)
        {
            float currDist = Vector3.Distance(path[i], transform.position);
            if (currDist < smallestDistance)
            {
                smallestDistance = currDist;
                closestIndex = i;
            }
        }
        currPathIndex = closestIndex;
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

            Rigidbody rb = GetComponent<Rigidbody>();

            // Fetch collision Point
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(transform.position - rb.velocity, rb.velocity, out hitInfo, rb.velocity.magnitude * 2, (1 << other.gameObject.layer)))
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
        AudioManager.Instance.Play("explosion");
        Destroy(gameObject);
    }

}
