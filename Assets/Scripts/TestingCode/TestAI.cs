using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestAI : MonoBehaviour
{
    [SerializeField]
    private AIFunc moveFunc;

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

    private Vector3 target;
    private bool followTarget = false;

    private Rigidbody rb;
    private void Start()
    {
        TargetPicker.targetReceiver += t => target = t; followTarget = true;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (followTarget)
        {
            Move();
        }
    }

    private void Move()
    {
        switch (moveFunc)
        {
            case AIFunc.Seek:
                rb.velocity = BasicAI.SteeringSeek(rb.position, rb.velocity, target, 
                    maxAcceleration, maxVelocity, Time.fixedDeltaTime);
                break;

            case AIFunc.SeekWithForce:
                Vector3 vel = BasicAI.SteeringSeek(rb.position, rb.velocity, target, 
                    maxAcceleration, maxVelocity, Time.fixedDeltaTime);
                rb.AddForce(BasicAI.VelocityToForce(vel, rb, Time.fixedDeltaTime));
                break;

            case AIFunc.Arrive:
                rb.velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, target,
                    slowDownRadius, arrivalRadius, maxAcceleration, maxVelocity, t2t, Time.fixedDeltaTime);
                break;

            case AIFunc.ArriveWithForce:
                Vector3 velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, target,
                    slowDownRadius, arrivalRadius, maxAcceleration, maxVelocity, t2t, Time.fixedDeltaTime);
                rb.AddForce(BasicAI.VelocityToForce(velocity, rb, Time.fixedDeltaTime));
                break;
        }
    }

    public enum AIFunc
    {
        Seek,
        SeekWithForce,
        Arrive,
        ArriveWithForce
    }

}
