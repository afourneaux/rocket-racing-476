using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// Component responsible for the behavior of the 
// AI characters that are racing with the player
[RequireComponent(typeof(Rigidbody), typeof(Vehicle))]
public class RacingAI : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float pathTargetDistance = 1.0f;

    [SerializeField]
    private LayerMask obstacleLayerMask;

    [SerializeField]
    private float maxRayDistance = 20.0f;

    [SerializeField]
    private float separateThreshold = 5.0f;

    [SerializeField]
    private float racingWeight = 1.0f;
    [SerializeField]
    private float separationWeight = 0.5f;

    private int pathIndex = 0;
    private Vehicle vehicleData;
    private bool startRace = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vehicleData = GetComponent<Vehicle>();
        RacerManager.AddRacer(rb);
    }


    private void FixedUpdate()
    {
        StartCoroutine(StartRace());
        if (startRace)
        {
            Vector3 force = Race();
            rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, vehicleData.GetRotationSpeed());
            rb.AddForce(force);
        }
    }

    // Returns the force to apply this physics update to follow the race track and avoid collision with obstacles
    private Vector3 Race()
    {
        List<Vector3> path = RaceTrack.GetPathPositions();
        pathIndex = BasicAI.GetNextPathIndex(path, rb.position, pathIndex);
        Vector3 followPath = BasicAI.SteeringFollowPath(path, pathIndex, pathTargetDistance,
            rb.position, rb.velocity, vehicleData.GetMaxAcceleration(), vehicleData.GetMaxVelocity(), Time.fixedDeltaTime);

        Vector3 racingVelocity = Vector3.zero;

        RaycastHit hitInfo = new RaycastHit();
        if (BasicAI.IsCollidingObstacle(rb.position, rb.velocity, transform.forward,
            vehicleData.GetWidth(), vehicleData.GetHeight(), maxRayDistance, obstacleLayerMask, out hitInfo))
        {
            racingVelocity = RacingAvoidObstacle(hitInfo, path, followPath);
        }
        else
        {
            racingVelocity = BasicAI.VelocityToForce(followPath, rb, Time.fixedDeltaTime);
        }
        return BasicAI.ClampVectorMagnitude(SeparationBehavior() * separationWeight
            + racingVelocity * racingWeight, vehicleData.GetMaxForce());
    }

    private Vector3 RacingAvoidObstacle(RaycastHit obstacleToAvoid, List<Vector3> path, Vector3 followPathVector)
    {
        int obstaclePathIndex = BasicAI.GetNextPathIndex(path, obstacleToAvoid.point, pathIndex);
        Vector3 seekTarget = path[obstaclePathIndex]
            + (path[obstaclePathIndex] - obstacleToAvoid.point).normalized * vehicleData.GetWidth();

        Vector3 seekOut = BasicAI.SteeringSeek(rb.position, rb.velocity, seekTarget,
            vehicleData.GetMaxAcceleration(), vehicleData.GetMaxVelocity(), Time.fixedDeltaTime);
        return BasicAI.VelocityToForce(seekOut, rb, Time.fixedDeltaTime);
    }

    private Vector3 SeparationBehavior()
    {
        List<Rigidbody> racers = RacerManager.GetRacers();
        Vector3 separationVelocity = Vector3.zero;
        foreach (Rigidbody racerRb in racers)
        {
            if (rb != racerRb)
            {
                separationVelocity += BasicAI.SteeringSeparate(rb.position, rb.velocity, racerRb.position,
                    racerRb.velocity, separateThreshold, vehicleData.GetMaxAcceleration(),
                    vehicleData.GetMaxVelocity(), Time.fixedDeltaTime);
            }
        }
        return separationVelocity;
    }

    IEnumerator StartRace()
    {
        yield return new WaitForSeconds(CountdownController.Instance.getCountdownTime());
        startRace = true;
    }
}
