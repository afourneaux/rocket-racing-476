using System.Collections.Generic;
using UnityEngine;

// Component responsible for the behavior of the 
// AI characters that are racing with the player
[RequireComponent(typeof(Rigidbody))]
public class RacingAI : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float maxVelocity = 20.0f;
    [SerializeField]
    private float maxForce = 10.0f;
    [SerializeField]
    private float rotationSpeed = 1.0f;

    [SerializeField]
    private float pathTargetDistance = 1.0f;

    [SerializeField]
    private float vehicleWidth = 2.0f;

    [SerializeField]
    private float vehicleHeight = 2.0f;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        RacerManager.AddRacer(rb);
    }


    private void FixedUpdate()
    {
        // Temporary to help demonstrate the obstacle avoiding behavior
        LineRenderer line = GetComponent<LineRenderer>();
        if (line != null)
        {
            line.enabled = false;
        }
        ///////////////////////////////

        Vector3 force = Race();
        rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
        rb.AddForce(force);
    }

    // Returns the force to apply this physics update to follow the race track and avoid collision with obstacles
    private Vector3 Race()
    {
        List<Vector3> path = RaceTrack.GetPathPositions();
        pathIndex = BasicAI.GetNextPathIndex(path, rb.position, pathIndex);
        Vector3 followPath = BasicAI.SteeringFollowPath(path, pathIndex, pathTargetDistance,
            rb.position, rb.velocity, maxAcceleration, maxVelocity, Time.fixedDeltaTime);

        Vector3 racingVelocity = Vector3.zero;

        RaycastHit hitInfo = new RaycastHit();
        if (BasicAI.IsCollidingObstacle(rb.position, rb.velocity, transform.forward,
            vehicleWidth, vehicleHeight, maxRayDistance, obstacleLayerMask, out hitInfo))
        {
            racingVelocity = RacingAvoidObstacle(hitInfo, path, followPath);
        }
        else
        {
            racingVelocity = BasicAI.VelocityToForce(followPath, rb, Time.fixedDeltaTime, maxForce);
        }
        return BasicAI.ClampVectorMagnitude(SeparationBehavior() * separationWeight 
            + racingVelocity * racingWeight, maxVelocity);
    }

    private Vector3 RacingAvoidObstacle(RaycastHit obstacleToAvoid, List<Vector3> path, Vector3 followPathVector)
    {
        int obstaclePathIndex = BasicAI.GetNextPathIndex(path, obstacleToAvoid.point, pathIndex);
        Vector3 seekTarget = path[obstaclePathIndex]
            + (path[obstaclePathIndex] - obstacleToAvoid.point).normalized * vehicleWidth;

        // Temporary, demonstrates the target to seek when avoiding obstacles. Will be removed for final build
        Vector3[] positions = { rb.position, seekTarget };
        LineRenderer line = GetComponent<LineRenderer>();
        if (line != null)
        {
            line.enabled = true;
            line.SetPositions(positions);
        }
        ////////////////////////////////

        Vector3 seekOut = BasicAI.SteeringSeek(rb.position, rb.velocity, seekTarget,
            maxAcceleration, maxVelocity, Time.fixedDeltaTime);
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
                    racerRb.velocity, separateThreshold, maxAcceleration, maxForce, Time.fixedDeltaTime);
            }
        }
        return separationVelocity;
    }

}
