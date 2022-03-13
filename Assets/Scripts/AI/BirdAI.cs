using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BirdAI : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 200.0f;
    [SerializeField]
    private float maxVelocity = 250.0f;
    [SerializeField]
    private float rotationSpeed = 1.0f;

    [SerializeField]
    private float chaseArrivalRadius = 1f;
    [SerializeField]
    private float chaseSlowDownRadius =3.0f;
    [SerializeField]
    private float chaseT2t = 0.5f;

    [SerializeField]
    private float resetArrivalRadius = 0.5f;
    [SerializeField]
    private float resetSlowDownRadius = 5.0f;
    [SerializeField]
    private float resetT2t = 1.0f;

    [SerializeField]
    private float shortestDistanceToRotate = 1.0f;


    private Vector3 startingPos;
    private Quaternion startingRotation;
    private Rigidbody rb;

    private  Vector3 target;
    private  bool resetting = true;

    private void Start()
    {
        startingPos = transform.position;
        startingRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        if (!resetting)
        {
            rb.velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, target, chaseSlowDownRadius, 
                chaseArrivalRadius, maxAcceleration, maxVelocity, chaseT2t, Time.fixedDeltaTime);
           
            if (rb.velocity != Vector3.zero && Vector3.Distance(rb.position, target) > shortestDistanceToRotate)
            {
                rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
            }
        }
       if (targetlist.Count == 0)
        {
            ResetUpdate();
        }
    }


    public void SetDestination(Vector3 position)
    {
        target = position;
        resetting = false;

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
            rb.rotation = Quaternion.Lerp(rb.rotation, startingRotation, Time.fixedDeltaTime);
        }
        else
        {
            rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
        }
    }


    //find closest AI
    public GameObject FindClosestAI()
    {
        GameObject[] AIs;
        AIs = GameObject.FindGameObjectsWithTag("AI");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject AI in AIs)
        {
            Vector3 diff = AI.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = AI;
                distance = curDistance;
            }
        }
        return closest;
    }
}
