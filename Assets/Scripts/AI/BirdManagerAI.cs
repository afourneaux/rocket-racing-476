using System.Collections.Generic;
using UnityEngine;


// High level AI responsible for the behavior of the individual birds and making sure that all the birds 
// in a section collaborate into bumping as many racers as possible.
[RequireComponent(typeof(BoxCollider))]
public class BirdManagerAI : MonoBehaviour
{
    private List<BirdTargetPair> birds = new List<BirdTargetPair>();
    private BoxCollider areaCollider;

    private List<TargetData> racersInRange = new List<TargetData>();

    // Used to prioritize which racer to bump first
    [SerializeField]
    private Transform endOfZone;
    [SerializeField]
    private float distanceFromCurrTargetWeight = 2.0f;
    [SerializeField]
    private float distanceFromNewTargetWeight = 0.5f;

    private void Start()
    {
        areaCollider = GetComponent<BoxCollider>();
        CollectBirds();
    }

    private void Update()
    {
        if (racersInRange.Count != 0)
        {
            UpdateBirdTargetPos();
        }
    }

    private void CollectBirds()
    {
        Collider[] colliders = Physics.OverlapBox(areaCollider.transform.position, areaCollider.size, transform.rotation);


        foreach (Collider currCollider in colliders)
        {
            BirdAI bird = currCollider.GetComponent<BirdAI>();
            if (bird != null)
            {
                BirdTargetPair pair = new BirdTargetPair();
                pair.bird = bird;
                birds.Add(pair);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vehicle v = other.GetComponent<Vehicle>();
        if (v != null)
        {
            TargetData data = new TargetData();
            data.racer = other.GetComponent<Rigidbody>();
            racersInRange.Add(data);
            AddRacer(data);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle v = other.GetComponent<Vehicle>();
        if (v != null)
        {
            RemoveRacer(other.GetComponent<Rigidbody>());
        }
    }

    private void RemoveRacer(Rigidbody racer)
    {
        // Remove racer from the list
        TargetData removedData = null;

        foreach (TargetData data in racersInRange)
        {
            if (data.racer == racer)
            {
                removedData = data;
                break;
            }
        }

        racersInRange.Remove(removedData);

        // If no racers in range reset the bird positions
        if (racersInRange.Count == 0)
        {
            ResetBirds();
            return;
        }

        // Re-assign birds
        for (int i = 0; i < removedData.birdsTargettingRacer.Count; i++)
        {
            // Pick target who has the least number of bird on them and who is the closest to the edge of the bird area
            TargetData leastTargettedRacer = GetMostFitTarget((x, y) => x < y, (x, y) => x < y);

            // Send one of the bird on that racer
            leastTargettedRacer.birdsTargettingRacer.Add(removedData.birdsTargettingRacer[i]);
            removedData.birdsTargettingRacer[i].targetRacer = leastTargettedRacer.racer;
        }
    }

    private void AddRacer(TargetData racer)
    {
        // If only one racer every bird target that racer
        if (racersInRange.Count == 1)
        {
            foreach(BirdTargetPair pair in birds)
            {
                pair.targetRacer = racer.racer;
                racer.birdsTargettingRacer.Add(pair);
            }
            return;
        }
        
        int numBirdsToReallocate =  birds.Count / racersInRange.Count;
        
        for (int i = 0; i < numBirdsToReallocate; i++)
        {
            ChangeBirdTarget(racer);
        }
    }

    private void ChangeBirdTarget(TargetData racer)
    {
        // Pick target which has the most birds targeting them and who is the farthest from the edge of the bird area.
        TargetData mostTargettedRacer = GetMostFitTarget((x, y) => x > y, (x, y) => x > y);

        // Check if first racer to enter
        if (mostTargettedRacer == null)
        {
            foreach (BirdTargetPair pair in birds)
            {
                pair.targetRacer = mostTargettedRacer.racer;
                mostTargettedRacer.birdsTargettingRacer.Add(pair);
            }
            return;
        }

        // Calculate which bird should be sent to new target
        float lowestScore = float.MaxValue;
        BirdTargetPair bestFitBird = null;

        foreach (BirdTargetPair pair in mostTargettedRacer.birdsTargettingRacer)
        {
            float currBirdScore = distanceFromCurrTargetWeight * Vector3.Distance(pair.bird.transform.position,
                pair.targetRacer.position) + distanceFromNewTargetWeight * Vector3.Distance(pair.bird.transform.position,
                racer.racer.position);

            if (currBirdScore < lowestScore)
            {
                lowestScore = currBirdScore;
                bestFitBird = pair;
            }
        }

        // Send best fit bird to target new racer
        mostTargettedRacer.birdsTargettingRacer.Remove(bestFitBird);
        racer.birdsTargettingRacer.Add(bestFitBird);
        bestFitBird.targetRacer = racer.racer;
    }

    // Delegates type used to reuse code when fetching the most fit racer. 
    // Used specifically by the GetMostFitTarget function.
    private delegate bool BirdCountOperator(int dataNum, int currNum);
    private delegate bool EdgeDistanceOperator(float currDist, float dataDist);

    private TargetData GetMostFitTarget(BirdCountOperator birdOperator, EdgeDistanceOperator edgeOperator)
    {
        int numBirdsOnSingleRacer = 0;
        TargetData mostFitTarget = null;

        foreach (TargetData data in racersInRange)
        {
            if (mostFitTarget == null)
            {
                mostFitTarget = data;
                numBirdsOnSingleRacer = mostFitTarget.birdsTargettingRacer.Count;
            }
            else if (data.birdsTargettingRacer.Count == numBirdsOnSingleRacer)
            {
                if (edgeOperator(Vector3.Distance(mostFitTarget.racer.position, endOfZone.position), 
                    Vector3.Distance(data.racer.position, endOfZone.position)))
                {
                    mostFitTarget = data;
                }
            }
            else if (birdOperator(data.birdsTargettingRacer.Count, numBirdsOnSingleRacer))
            {
                numBirdsOnSingleRacer = data.birdsTargettingRacer.Count;
                mostFitTarget = data;
            }
        }
        return mostFitTarget;
    }

    private void UpdateBirdTargetPos()
    {
        foreach (BirdTargetPair pair in birds)
        {
            /*
            // Find the next expected position of the target racer and go to that position
            Vector3 targetPos = pair.bird.transform.position + pair.targetRacer.velocity 
                * (Vector3.Distance(pair.bird.transform.position, pair.targetRacer.position) 
                / pair.bird.GetMaxVelocity());
            pair.bird.SetDestination(targetPos);
            */
            pair.bird.SetDestination(pair.targetRacer.position);
        }
    }

    private void ResetBirds()
    {
        foreach(BirdTargetPair pair in birds)
        {
            pair.bird.ResetPosition();
        }
    }

    private class BirdTargetPair
    {
        public BirdAI bird;
        public Rigidbody targetRacer = null;
    }

    private class TargetData
    {
        public Rigidbody racer;
        public List<BirdTargetPair> birdsTargettingRacer = new List<BirdTargetPair>();
    }
}
