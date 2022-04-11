using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsRoot : MonoBehaviour
{
    HashSet<int> activeCollisions;
    Rigidbody rb;

    public float Restitution = 1f;
    const float RAYCAST_TOLERANCE = 0.5f;
    const float VELOCITY_THRESHOLD = 1f;
    const float ANGLE_THRESHOLD = 5f;
    const float INTERPENETRATION_BUFFER = 0.5f;

    void Start() {
        Time.timeScale = 0.5f;
        activeCollisions = new HashSet<int>();
        rb = GetComponent<Rigidbody>();

        // Propagate the CollisionsSub script to all child transforms containing a collider
        foreach(Transform child in transform) {
            if (child.GetComponent<Collider>() != null) {
                child.gameObject.AddComponent<CollisionsSub>();
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
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
        List<Vector3> points = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < collision.contactCount; i++) {
            ContactPoint point = collision.GetContact(i);
            points.Add(point.point);
            normals.Add(point.normal);
            Debug.DrawLine(source.bounds.center, point.point, Color.green, 2f);
        }
        Vector3 contactPointOnOther = Vector3Average(points);
        Vector3 contactNormal = Vector3Average(normals);
        Vector3 distanceToContactPoint = contactPointOnOther - source.bounds.center;

        int layer = source.gameObject.layer;
        source.gameObject.layer = 7;
        int layerMask = 1 << 7;

        RaycastHit hitInfo;
        Physics.Raycast(source.bounds.center + distanceToContactPoint * 5, -distanceToContactPoint.normalized, out hitInfo, distanceToContactPoint.magnitude * 5, layerMask);
        
        Vector3 interpenetration = contactPointOnOther - hitInfo.point;

        source.gameObject.layer = layer;

        if (rb.velocity.magnitude <= VELOCITY_THRESHOLD) {
            rb.transform.position += Vector3.Project(interpenetration, contactNormal) + (contactNormal * INTERPENETRATION_BUFFER);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            rb.transform.position += interpenetration + (contactNormal * INTERPENETRATION_BUFFER);
            Vector3 projection = Vector3.Project(-rb.velocity, contactNormal);
            rb.AddForce((projection) * (1 + Restitution), ForceMode.Impulse);

            float angle = Vector3.Angle(-distanceToContactPoint, contactNormal);
            if (angle >= ANGLE_THRESHOLD) {
                Vector3 rotateAxis = Vector3.Cross(distanceToContactPoint, contactNormal);
                float torque = angle / 180f * rb.velocity.magnitude;
                rb.AddTorque(torque * rotateAxis, ForceMode.Impulse);
            }
        }
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
