using System.Collections.Generic;
using UnityEngine;

public class BasicAI 
{
    // Returns the velocity of the character this physics update
    public static Vector3 SteeringArrive(Vector3 currPos, Vector3 previousVelocity, 
        Vector3 targetPos, float slowDownRadius, float arrivalRadius, 
        float maxAcceleration, float maxVelocity, float t2t, float timeStep)
    {
        float distance = Vector3.Distance(currPos, targetPos);
        if (distance <= arrivalRadius)
        {
            return Vector3.zero;
        }
        else if (distance <= slowDownRadius)
        {
            float goalVelocity = Mathf.Min(maxVelocity, distance / t2t);
            float accelerationMagnitude = (goalVelocity - previousVelocity.magnitude) / t2t;
            Vector3 acceleration = accelerationMagnitude * (targetPos - currPos).normalized;
            return previousVelocity + acceleration * timeStep;
        }
        return SteeringSeek(currPos, previousVelocity, targetPos, maxAcceleration, maxVelocity, timeStep);
    }

    // Returns the velocity of the character this physics update
    public static Vector3 SteeringSeek(Vector3 currPos, Vector3 previousVelocity, 
        Vector3 targetPos, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 acceleration = (targetPos - currPos).normalized * maxAcceleration;
        Vector3 velocity = previousVelocity + acceleration * timeStep;
        if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
        return velocity;
    }


    /* returns the velocity to apply to the character this frame to follow the provided path
     * 
     * the path is an ordered list of positions describing the path
     * the currPathIndex is the index of the current path position in the provided list
     * and the targetDistance is the distance of the next position in the path to pick. the target 
     * will be the first position in the path which is smaller than this distance 
     * and which is greater than the current path position. If no such distance is 
     * found, the target will be the next position in the path starting from the current position.
    */
    public static Vector3 FollowPath(List<Vector3> path, int currPathIndex, float targetDistance, 
        Vector3 currPos, Vector3 previousVelocity, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 currPathPos = path[currPathIndex];
        Vector3 seekTarget = Vector3.zero;
        for (int i = currPathIndex + 1; i < path.Count; i++)
        {
            if (Vector3.Distance(currPathPos, path[i]) > targetDistance)
            {
                if (i == currPathIndex + 1)
                {
                    seekTarget = path[i];
                }
                else
                {
                    seekTarget = path[i - 1];
                }
                break;
            }
        }

        return SteeringSeek(currPos, previousVelocity, seekTarget, maxAcceleration, maxVelocity, timeStep);
    }

    // Calculates the force to apply to a rigidbody given it's desired velocity and it's current velocity
    public static Vector3 VelocityToForce(Vector3 velocityThisUpdate, Rigidbody rb, float timeStep)
    {
        Vector3 acceleration = (velocityThisUpdate - rb.velocity) / timeStep;
        return acceleration * rb.mass;
    }
}
