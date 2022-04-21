using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsRoot : MonoBehaviour
{
    HashSet<int> activeCollisions;
    Rigidbody rb;
    Collider myCollider;
    bool hasCollided = false;
    bool isResting = false;

    Vector3 deepestPenetration;
    Vector3 deepestNormal;
    Vector3 deepestContactPoint;

    public float Restitution = 1f;
    public float Friction = 0.8f;   // 0 for perfectly coarse, 1 for perfectly smooth
    const float VELOCITY_THRESHOLD_LINEAR = 1f;
    const float VELOCITY_THRESHOLD_ANGULAR = 1f;

    void Awake() {
        activeCollisions = new HashSet<int>();
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        // Propagate the CollisionsSub script to all child transforms containing a collider
        foreach(Transform child in transform) {
            if (child.GetComponent<Collider>() != null) {
                child.gameObject.AddComponent<CollisionsSub>();
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        if (activeCollisions.Count == 0) {
            EnableChildColliders();
        }
        activeCollisions.Add(collider.GetInstanceID());
    }

    void OnTriggerExit(Collider collider) {
        activeCollisions.Remove(collider.GetInstanceID());
        if (activeCollisions.Count == 0) {
            DisableChildColliders();
        }
    }

    void EnableChildColliders() {
        foreach (Transform child in transform) {
            CollisionsSub colSub = child.GetComponent<CollisionsSub>();
            Collider col = child.GetComponent<Collider>();
            if (colSub != null) {
                colSub.EnableChildColliders();
            }
            if (col != null) {
                col.enabled = true;
            }
        }
    }

    void DisableChildColliders() {
        foreach (Transform child in transform) {
            CollisionsSub colSub = child.GetComponent<CollisionsSub>();
            Collider col = child.GetComponent<Collider>();
            if (colSub != null) {
                colSub.DisableChildColliders();
            }
            if (col != null) {
                col.enabled = false;
            }
        }
    }

    public void HandleCollision(Collider source, Collision collision, bool resting) {
        // Get the collision point
        List<Vector3> points = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < collision.contactCount; i++) {
            // Average all contact points and contact normals to estimate our global contact normal
            ContactPoint point = collision.GetContact(i);
            points.Add(point.point);
            normals.Add(point.normal);
        }
        Vector3 contactPointOnOther = Vector3Average(points);
        Vector3 contactNormal = Vector3Average(normals);

        // Ignore separating contacts
        if (Vector3.Dot(rb.velocity, contactNormal) > 0) {
            return;
        }

        hasCollided = true;
        Vector3 distanceToContactPoint = contactPointOnOther - source.bounds.center;

        // Perform physics checks only on the object in question
        int layer = source.gameObject.layer;
        source.gameObject.layer = 7;
        int layerMask = 1 << 7;

        // Get the penetration point and depth by shooting a raycast back at ourselves from the direction of the contact point
        Vector3 testPoint = contactPointOnOther;
        do {
            testPoint += distanceToContactPoint;
        } while (Physics.CheckSphere(testPoint, 0.01f, layerMask));

        RaycastHit hitInfo;
        Physics.Raycast(testPoint, -distanceToContactPoint.normalized, out hitInfo, (source.bounds.center - testPoint).magnitude, layerMask);
        Vector3 interpenetration = contactPointOnOther - hitInfo.point;

        if (Vector3.Dot(interpenetration, contactNormal) < 0) {
            interpenetration = -interpenetration;
        }

        // Resolve only the deepest penetrating object
        if (interpenetration.magnitude > deepestPenetration.magnitude) {
            deepestPenetration = interpenetration;
            deepestNormal = contactNormal;
            deepestContactPoint = hitInfo.point;
            isResting = resting;
        }

        // Restore the game object's layer to its original state
        source.gameObject.layer = layer;
    }

    void FixedUpdate() {
        if (hasCollided == false) {
            return;
        }

        // Determine linear and angular component of nonlinear projection
        float inverseMass = 1f / rb.mass;

        Vector3 qrel = transform.position - deepestContactPoint;

        Vector3 axis = Vector3.Cross(qrel, deepestNormal).normalized;
        Vector3 inverseInertia = new Vector3(1f / rb.inertiaTensor.x, 1f / rb.inertiaTensor.y, 1f / rb.inertiaTensor.z);

        Vector3 deltaTheta = Vector3.Project(axis, inverseInertia);
        float angularInertia = Vector3.Dot(Vector3.Cross(deltaTheta, qrel), deepestNormal);

        float totalInertia = angularInertia + inverseMass;

        float deltaLinear = deepestPenetration.magnitude * (inverseMass / totalInertia);
        float deltaAngular = deepestPenetration.magnitude * (angularInertia / totalInertia);

        Quaternion rotation = Quaternion.AngleAxis(deltaAngular / angularInertia, deltaTheta);

        // Calculate the impulse for bouncing off of an object
        Vector3 desiredVelocityChange = (-rb.velocity) * (1 + Restitution);

        Vector3 deltaThetaImpulse = Vector3.Project(Vector3.Cross(deepestNormal, qrel), inverseInertia);

        
        Vector3 rotationalVelocityChange = Vector3.Cross(deltaThetaImpulse, qrel);
        float rotationalVelocity = Vector3.Dot(rotationalVelocityChange, deepestNormal);

        float overallVelocityChange = rotationalVelocity + inverseMass;

        Vector3 impulse = desiredVelocityChange / overallVelocityChange;

        // Begin by applying nonlinear projection to move the colliding objects out of one another, then bounce the object off of the contact.
        // If the object is moving slower than a certain threshold, assume it is at rest and stop it
        if (isResting || rb.velocity.magnitude <= VELOCITY_THRESHOLD_LINEAR) {
            rb.MovePosition(rb.position + Vector3.Project(deepestPenetration, deepestNormal));
            rb.velocity = Vector3.zero;
        } else {
            rb.MovePosition(rb.position + (deepestNormal * deltaLinear));
            rb.AddForce(rb.mass * Vector3.Project(desiredVelocityChange, deepestNormal), ForceMode.Impulse);
            rb.velocity *= Friction;
        }

        // Prevent the impulse from overcorrecting the required angle
        float maxAngle = Vector3.Angle(qrel, deepestNormal);
        float maxAngularVelocity = rb.maxAngularVelocity;
        rb.maxAngularVelocity = maxAngle;

        if (isResting || rb.angularVelocity.magnitude <= VELOCITY_THRESHOLD_ANGULAR) {
            rb.angularVelocity = Vector3.zero;
        } else {
            rb.MoveRotation(rb.rotation * rotation);
            Vector3 rotationalCounterImpulse = rb.mass * Vector3.Project(-rb.angularVelocity, deltaThetaImpulse) * (1 + Restitution);
            rb.AddTorque(rotationalCounterImpulse, ForceMode.Impulse);
        }

        rb.AddTorque(deltaThetaImpulse, ForceMode.Impulse);
        
        rb.maxAngularVelocity = maxAngularVelocity;
        
        hasCollided = false;
        isResting = false;
        deepestNormal = Vector3.zero;
        deepestPenetration = Vector3.zero;
        deepestContactPoint = Vector3.zero;
    }

    Vector3 ScaleVector(Vector3 baseVector, Vector3 scaleVector) {
        return new Vector3(baseVector.x * scaleVector.x, baseVector.y * scaleVector.y, baseVector.z * scaleVector.z);
    }

    Vector3 Vector3Average(List<Vector3> points) {
        int count = 0;
        float x = 0f;
        float y = 0f;
        float z = 0f;

        foreach (Vector3 point in points) {
            count++;
            x += point.x;
            y += point.y;
            z += point.z;
        }

        return new Vector3(x / (float)count, y / (float)count, z / (float)count);
    }
}
