using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class BirdAI : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float maxVelocity = 20.0f;
    [SerializeField]
    private float rotationSpeed = 1.0f;
    [SerializeField]
    private float resetRotationSpeedOnArrival = 5.0f;

    [SerializeField]
    private float chaseArrivalRadius = 0.5f;
    [SerializeField]
    private float chaseSlowDownRadius = 1.0f;
    [SerializeField]
    private float chaseT2t = 0.5f;

    [SerializeField]
    private float resetArrivalRadius = 0.5f;
    [SerializeField]
    private float resetSlowDownRadius = 5.0f;
    [SerializeField]
    private float resetT2t = 1.0f;
    [SerializeField]
    private float resetArrivalAngle = 10.0f;

    [SerializeField]
    private float shortestDistanceToRotate = 1.0f;
    [SerializeField]
    private float minVelocityOnReset = 2.0f;

    [SerializeField]
    private float resetSlideSpeed = 0.5f;
    private Animator anim;

    private Vector3 startingPos;
    private Quaternion startingRotation;
    private Vector3 startingForwardVector;
    private Rigidbody rb;

    private Vector3 target;
    private bool resetting = true;
    private bool isReset = true;
    private float BirdSoundInterval = 1.5f;

    private void Start()
    {
        startingPos = transform.position;
        startingRotation = transform.rotation;
        startingForwardVector = transform.forward;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isReset == true)
        {
            transform.position = Vector3.Lerp(transform.position, startingPos, resetSlideSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, startingRotation, resetSlideSpeed);
            return;
        }

        if (!resetting)
        {
            BirdSoundInterval -= Time.deltaTime;
            if (BirdSoundInterval <= 0)
            {
                AudioManager.Instance.Play("bird");
                BirdSoundInterval = 1f;
            }
            rb.velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, target, chaseSlowDownRadius, 
                chaseArrivalRadius, maxAcceleration, maxVelocity, chaseT2t, Time.fixedDeltaTime);

            float distanceToTarget = Vector3.Distance(rb.position, target);
            if (rb.velocity != Vector3.zero && Vector3.Distance(rb.position, target) > shortestDistanceToRotate)
            {
                rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
            }
        }
        else
        {
            ResetUpdate();
        }
    }

    public float GetMaxVelocity() { return maxVelocity; }

    public void SetDestination(Vector3 position)
    {
        target = position;
        resetting = false;
        anim.SetBool("IsFlying", true);
        isReset = false;
    }

    public void ResetPosition()
    {
        resetting = true;
    }

    private void ResetUpdate()
    {
        rb.velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, startingPos,
            resetSlowDownRadius, resetArrivalRadius, maxAcceleration, maxVelocity, resetT2t, Time.fixedDeltaTime);

        if (rb.velocity == Vector3.zero)
        {
            //Rotate only and slide to the start pos
            rb.position = Vector3.Lerp(rb.position, startingPos, resetSlideSpeed);

            float angle = Vector3.Angle(transform.forward, startingForwardVector);
            if (Vector3.Angle(transform.forward, startingForwardVector) > resetArrivalAngle)
            {
                rb.rotation = Quaternion.Lerp(rb.rotation, startingRotation, 
                    Mathf.Clamp(Time.fixedDeltaTime * resetRotationSpeedOnArrival, 0.0f, 1.0f));
            }
            else
            {
                Reset();
            }
        }
        else
        {
            //moving and rotating
            rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
        }
    }

    private void Reset()
    {
        isReset = true;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        anim.SetBool("IsFlying", false);
    }

    public bool getResetting()
    {
        return resetting;
    }
}
