using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestAI : MonoBehaviour
{
    [SerializeField]
    private AIFunc moveFunc;

    [SerializeField]
    private float maxForce = 10.0f;

    [SerializeField]
    private float maxVelocity = 10.0f;
    [SerializeField]
    private float maxAcceleration = 5.0f;

    [SerializeField]
    private float slowDownRadius = 7.0f;
    [SerializeField]
    private float arrivalRadius = 2.0f;

    [SerializeField]
    private float t2t = 0.55f;

    [SerializeField]
    private float pathTargetDistance = 1.0f;

    private Vector3 target;
    private bool followTarget = false;

    [SerializeField]
    private Vector3 currPathPos;

    private int pathIndex = 0;

    private Rigidbody rb;
    private void Start()
    {
        TargetPicker.targetReceiver += t => target = t; followTarget = true;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        List<Vector3> path = PathManager.GetPath();
        pathIndex = BasicAI.GetNextPathIndex(path, rb.position, pathIndex);

        currPathPos = path[pathIndex];

        switch (moveFunc)
        {
            case AIFunc.FollowPath:
                rb.velocity = BasicAI.SteeringFollowPath(path, pathIndex, pathTargetDistance, rb.position,
                    rb.velocity, maxAcceleration, maxVelocity, Time.fixedDeltaTime);
                break;

            case AIFunc.FollowPathWithForce:
                Vector3 velocity = BasicAI.SteeringFollowPath(PathManager.GetPath(), pathIndex, pathTargetDistance, 
                    rb.position, rb.velocity, maxAcceleration, maxVelocity, Time.fixedDeltaTime);
                rb.AddForce(BasicAI.VelocityToForce(velocity, rb, Time.fixedDeltaTime, maxForce));
                break;
        }
    }

    public enum AIFunc
    {
        FollowPath,
        FollowPathWithForce
    }

}
