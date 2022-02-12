using System.Collections.Generic;
using UnityEngine;

public class BasicAI 
{
    public static Quaternion SteeringLookWhereYouAreGoing(Quaternion currRotation, Vector3 velocity, float rotationSpeed)
    {
        if (velocity == Vector3.zero)
        {
            return currRotation;
        }
        Quaternion goalRotation = Quaternion.LookRotation(velocity);
        return Quaternion.LerpUnclamped(currRotation, goalRotation, rotationSpeed);
    }

    /* public static Quaternion KinematicLookWhereYouAreGoing(Quaternion currRotation, Vector3 velocity, float rotationSpeed)
     {
         if (velocity == Vector3.zero)
         {
             return currRotation;
         }
         return Quaternion.LookRotation(velocity);
     }

     public static  Quaternion SteeringLookWhereYouAreGoing(Quaternion currRotation, Vector3 velocity, float rotationSpeed, Vector3 forwardVector)
     {
         Vector3 from = Vector3.ProjectOnPlane(forwardVector, Vector3.up);
         Vector3 to = KinematicLookWhereYouAreGoing(currRotation, velocity, rotationSpeed) * Vector3.forward;
         float angleY = Vector3.SignedAngle(from, to, Vector3.up);
         return Quaternion.AngleAxis(angleY, Vector3.up);
     }*/


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

    // Returns the velocity of the character this physics update
    public static Vector3 SteeringFlee(Vector3 currPos, Vector3 previousVelocity,
        Vector3 targetPos, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 desiredVelocity = KinematicFlee(currPos, targetPos, maxVelocity);
        Vector3 acceleration = ClampVectorMagnitude(desiredVelocity - previousVelocity, maxAcceleration);
        Vector3 velocity = previousVelocity + acceleration * timeStep;
        return ClampVectorMagnitude(velocity, maxVelocity);
    }

    public static Vector3 KinematicSeek(Vector3 currPos, Vector3 targetPos, float maxVelocity)
    {
        Vector3 direction = targetPos - currPos;
        return direction.normalized * maxVelocity;
    }

    public static Vector3 KinematicFlee(Vector3 currPos, Vector3 targetPos, float maxVelocity)
    {
        Vector3 direction = currPos - targetPos;
        return direction.normalized * maxVelocity;
    }

    // Returns the velocity of to apply to separate from the target this update
    public static Vector3 SteeringSeparate(Vector3 currPos, Vector3 currVelocity, Vector3 targetToAvoidPos, 
        Vector3 targetVelocity, float distanceThreshold, float maxAcceleration, float maxVelocity, float timeStep)
    {
        float distance = Vector3.Distance(currPos, targetToAvoidPos);
        if (distance >= distanceThreshold)
        {
            return Vector3.zero;
        }

        float strength = maxAcceleration * (distanceThreshold - distance) / distanceThreshold;
        return SteeringEvade(currPos, currVelocity, targetToAvoidPos, targetVelocity, maxAcceleration, strength, timeStep);
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
    public static Vector3 SteeringFollowPath(List<Vector3> path, int currPathIndex, float targetDistance, 
        Vector3 currPos, Vector3 previousVelocity, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 currPathPos = path[currPathIndex];
        Vector3 seekTarget = path[GetTargetPathPos(path, currPathIndex, targetDistance, currPos)];

        return SteeringSeek(currPos, previousVelocity, seekTarget, maxAcceleration, maxVelocity, timeStep);
    }

    public static int GetTargetPathPos(List<Vector3> path, int currPathIndex, float targetDistance, Vector3 currPos)
    {
        Vector3 currPathPos = path[currPathIndex];
        for (int i = currPathIndex + 1; i < path.Count; i++)
        {
            if (Vector3.Distance(currPathPos, path[i]) > targetDistance)
            {
                if (i == currPathIndex + 1)
                {
                    return i;
                }
                else
                {
                    return i - 1;
                }
            }
        }
        return path.Count - 1;
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

    public static Vector3 AvoidCollisionTarget(Vector3 currPos, Vector3 currVelocity, Vector3 targetPos, 
        Vector3 targetVelocity, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 currRelativePos = targetPos - currPos;
        Vector3 relativeVelocity = targetVelocity - currVelocity;
        float timeOfClosestApproach = -(Vector3.Dot(currRelativePos, relativeVelocity) / relativeVelocity.sqrMagnitude);

        //no incoming collisions to avoid
        if (timeOfClosestApproach < 0.0f)
        {
            return Vector3.zero;
        }

        Vector3 currPosClosest = currPos + currVelocity * timeOfClosestApproach;
        Vector3 targetPosClosest = targetPos + targetVelocity * timeOfClosestApproach;

        return SteeringEvade(currPosClosest, currVelocity, targetPosClosest, targetVelocity, 
            maxAcceleration, maxVelocity, timeStep);
    }

    // Checks for collision using 5 rays, 2 parallel horizontally, 2 parallel vertically 
    // and one in the middle aimed towards where the character is moving
    public static Vector3 AvoidCollisionObstacle(Vector3 currPos, Vector3 velocity, float maxAcceleration, 
        float maxVelocity, float timeStep, Vector3 forwardVector, float characterWidth, float characterHeight, float rayMaxDistance, 
        float distFromNormal, LayerMask mask)
    {
        // Check center ray first
        RaycastHit hitInfo = new RaycastHit();
        if (IsCollidingObstacle(currPos, velocity, forwardVector, characterWidth, characterHeight, 
            rayMaxDistance, mask, out hitInfo))
        {
            Vector3 seekTarget = hitInfo.normal * distFromNormal;
            return SteeringSeek(currPos, velocity, seekTarget, maxAcceleration, maxVelocity, timeStep);
        }
        
        // If nothing hit no need to avoid anything
        return Vector3.zero;
    }

    // Checks if the AI character is colliding with an obstacle and stores the information about the first hit by 
    // the rays into the provided RaycastHit. Returns true if a collision occured, false otherwise
    public static bool IsCollidingObstacle(Vector3 currPos, Vector3 velocity, Vector3 forwardVector, 
        float characterWidth, float characterHeight, float rayMaxDistance, LayerMask mask, out RaycastHit hitInfo)
    {
        // Check center ray first
        Ray centerRay = new Ray(currPos, velocity);

        if (Physics.Raycast(centerRay, out hitInfo, rayMaxDistance, mask))
        {
            return true;
        }

        // If no hit check first parallel horizontal ray
        Vector3 horizontalVector = Vector3.Cross(forwardVector, Vector3.up).normalized;
        Ray parallelHorizontalRay1 = new Ray(currPos + horizontalVector * (characterWidth / 2.0f), velocity);

        if (Physics.Raycast(parallelHorizontalRay1, out hitInfo, rayMaxDistance, mask))
        {
            return true;
        }

        // If still no hit check second parallel ray
        Ray parallelHorizontalRay2 = new Ray(currPos - horizontalVector * (characterWidth / 2.0f), velocity);

        if (Physics.Raycast(parallelHorizontalRay2, out hitInfo, rayMaxDistance, mask))
        {
            return true;
        }

        Vector3 planeVector = Vector3.Angle(Vector3.right, forwardVector) 
            > Vector3.Angle(Vector3.forward, forwardVector) ? Vector3.right : Vector3.forward;
        Vector3 verticalVector = Vector3.Cross(forwardVector, planeVector);

        Ray parallelVecticalRay1 = new Ray(currPos + verticalVector * (characterHeight / 2.0f), velocity);

        if (Physics.Raycast(parallelVecticalRay1, out hitInfo, rayMaxDistance, mask))
        {
            return true;
        }


        Ray parallelVecticalRay2 = new Ray(currPos - verticalVector * (characterHeight / 2.0f), velocity);

        if (Physics.Raycast(parallelVecticalRay2, out hitInfo, rayMaxDistance, mask))
        {
            return true;
        }

        return false;
    }

    // Checks if the AI character is colliding with an obstacle. Returns true if a collision occured, false otherwise
    public static bool IsCollidingObstacle(Vector3 currPos, Vector3 velocity, Vector3 forwardVector,
        float characterWidth, float characterHeight, float rayMaxDistance, LayerMask mask)
    {
        RaycastHit hit = new RaycastHit();
        return IsCollidingObstacle(currPos, velocity, forwardVector, characterWidth, 
            characterHeight, rayMaxDistance, mask, out hit);
    }

    public static Vector3 SteeringEvade(Vector3 currPos, Vector3 currVelocity, 
        Vector3 targetPos, Vector3 targetVelocity, float maxAcceleration, float maxVelocity, float timeStep)
    {
        Vector3 fleeTarget = targetPos + targetVelocity * (Vector3.Distance(currPos, targetPos) / maxVelocity);
        return SteeringFlee(currPos, currVelocity, fleeTarget, maxAcceleration, maxVelocity, timeStep);
    }

    public static int GetNextPathIndex(List<Vector3> path, Vector3 currPos, int prevIndex)
    {
        float smallestDistance = float.MaxValue;
        int nextIndex = prevIndex;
        bool foundSmaller = false;
        for (int i = prevIndex; i < path.Count; i++)
        {
            float currDist = Vector3.Distance(path[i], currPos);
            if (currDist < smallestDistance)
            {
                smallestDistance = currDist;
                nextIndex = i;
                foundSmaller = true;
            }
            else if (foundSmaller)
            {
                break;
            }
        }
        return nextIndex;
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
