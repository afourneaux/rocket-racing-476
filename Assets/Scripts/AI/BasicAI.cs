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
            Vector3 desiredVelocity = KinematicArrive(currPos, targetPos, slowDownRadius, 
                arrivalRadius, t2t, maxVelocity);
            Vector3 acceleration = ClampVectorMagnitude(desiredVelocity - previousVelocity, maxAcceleration);
            Vector3 velocity = previousVelocity + acceleration * timeStep;
            return ClampVectorMagnitude(velocity, maxVelocity);
        }
        return SteeringSeek(currPos, previousVelocity, targetPos, maxAcceleration, maxVelocity, timeStep);
    }

    // Returns the velocity of the character this physics update
    public static Vector3 SteeringSeek(Vector3 currPos, Vector3 previousVelocity, 
        Vector3 targetPos, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 desiredVelocity = KinematicSeek(currPos, targetPos, maxVelocity);
        Vector3 acceleration = ClampVectorMagnitude(desiredVelocity - previousVelocity, maxAcceleration);
        Vector3 velocity = previousVelocity + acceleration * timeStep;
        return ClampVectorMagnitude(velocity, maxVelocity);
    }

    public static Vector3 KinematicSeek(Vector3 currPos, Vector3 targetPos, float maxVelocity)
    {
        Vector3 direction = targetPos - currPos;
        return direction.normalized * maxVelocity;
    }

    public static Vector3 KinematicArrive(Vector3 currPos, Vector3 targetPos, float slowDownRadius, 
        float arrivalRadius, float t2t, float maxVelocity)
    {
        float distance = Vector3.Distance(currPos, targetPos);
        if (distance <= arrivalRadius)
        {
            return Vector3.zero;
        }
        else if (distance <= slowDownRadius)
        {
            float desiredSpeed = Mathf.Min(maxVelocity, distance / t2t);
            return (targetPos - currPos).normalized * desiredSpeed;
        }
        return KinematicSeek(currPos, targetPos, maxVelocity);
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

    // Calculates the force to apply to a rigidbody to reach it's desired velocity given it's current velocity
    public static Vector3 VelocityToForce(Vector3 velocityThisUpdate, Rigidbody rb, float timeStep)
    {
        Vector3 acceleration = (velocityThisUpdate - rb.velocity) / timeStep;
        return acceleration * rb.mass;
    }

    // Calculates the force to apply to a rigidbody to reach it's desired velocity given it's current velocity. 
    // Will automatically clamp the magnitude of the returned force by the maxForce provided
    public static Vector3 VelocityToForce(Vector3 velocityThisUpdate, Rigidbody rb, float timeStep, float maxForce)
    {
        return ClampVectorMagnitude(VelocityToForce(velocityThisUpdate, rb, timeStep), maxForce);
    }

    public static Vector3 ClampVectorMagnitude(Vector3 v, float maxMagnitude)
    {
        if (v.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            return v.normalized * maxMagnitude;
        }
        return v;
    }
}
