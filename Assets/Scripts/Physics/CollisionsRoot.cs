using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsRoot : MonoBehaviour
{
    HashSet<int> activeCollisions;
    Rigidbody rb;
    Collider myCollider;
    bool hasCollided = false;

    Vector3 deepestPenetration;
    Vector3 deepestNormal;
    Vector3 deepestContactPoint;

    public float Restitution = 1f;
    const float VELOCITY_THRESHOLD = 1f;
    const float ANGLE_THRESHOLD = 5f;

    void Start() {
        //Time.timeScale = 0.1f;
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

    public void HandleCollision(Collider source, Collision collision) {
        hasCollided = true;
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
        }

        // Restore the game object's layer to its original state
        source.gameObject.layer = layer;
    }

    void FixedUpdate() {
        if (hasCollided == false) {
            return;
        }

        // Apply all relevant forces. If the object is moving slower than a certain threshold, assume it is at rest and stop it
        if (rb.velocity.magnitude <= VELOCITY_THRESHOLD) {
            rb.transform.position += Vector3.Project(deepestPenetration, deepestNormal);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            rb.transform.position += Vector3.Project(deepestPenetration, deepestNormal);
            Vector3 projection = Vector3.Project(-rb.velocity, deepestNormal);  // TODO: Linear component instead of rb.velocity
            rb.AddForce(rb.mass * (projection) * (1 + Restitution), ForceMode.Impulse);
        }

        float angle = Vector3.Angle(myCollider.bounds.center - deepestContactPoint, deepestNormal);
        if (angle >= ANGLE_THRESHOLD) {
            Vector3 rotateAxis = Vector3.Cross(deepestPenetration.normalized, deepestNormal).normalized;
            float torque = angle / 180f;    // TODO: Angular component
            rb.AddTorque(rb.mass * torque * rotateAxis, ForceMode.Impulse);
        }
        hasCollided = false;
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
