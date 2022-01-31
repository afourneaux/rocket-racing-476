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
    private float pathTargetDistance = 1.0f;

    [SerializeField]
    private float vehicleWidth = 2.0f;

    [SerializeField]
    private float avoidDistanceFromNormal = 3.0f;

    [SerializeField]
    private LayerMask obstacleLayerMask;

    [SerializeField]
    private float maxRayDistance = 20.0f;

    [SerializeField]
    private float followPathWeight = 0.7f;

    [SerializeField]
    private float avoidObstaclesWeight = 0.2f;

    private int pathIndex = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    private void FixedUpdate()
    {
        Vector3 force = Race();
        rb.AddForce(force);
    }

    // Returns the force to apply this physics update to follow the race track and avoid collision with obstacles
    private Vector3 Race()
    {
        Vector3 avoidObstacles = BasicAI.AvoidCollisionObstacle(rb.position, rb.velocity, maxAcceleration, 
            maxVelocity, Time.fixedDeltaTime, transform.forward, vehicleWidth, maxRayDistance, 
            avoidDistanceFromNormal, obstacleLayerMask);

        List<Vector3> path = RaceTrack.GetPathPositions();
        pathIndex = BasicAI.GetNextPathIndex(path, rb.position, pathIndex);
        Vector3 followPath = BasicAI.SteeringFollowPath(path, pathIndex, pathTargetDistance, 
            rb.position, rb.velocity, maxAcceleration, maxVelocity, Time.fixedDeltaTime);

        Vector3 desiredVelocity = avoidObstacles * avoidObstaclesWeight + followPath * followPathWeight;
        return BasicAI.VelocityToForce(BasicAI.ClampVectorMagnitude(desiredVelocity, maxVelocity), 
            rb, Time.fixedDeltaTime, maxForce);
    }

}
