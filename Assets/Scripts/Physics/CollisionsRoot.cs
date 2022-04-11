using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsRoot : MonoBehaviour
{
    HashSet<int> activeCollisions;
    Rigidbody rb;

    public float Restitution = 1f;
    const float RAYCAST_TOLERANCE = 0.5f;
    const float VELOCITY_THRESHOLD = 2f;
    const float INTERPENETRATION_BUFFER = 0.03f;

    void Start() {
        //Time.timeScale = 0.05f;
        activeCollisions = new HashSet<int>();
        rb = GetComponent<Rigidbody>();

        // Propagate the CollisionsSub script to all child transforms containing a collider
        foreach(Transform child in transform) {
            if (child.GetComponent<Collider>() != null) {
                child.gameObject.AddComponent<CollisionsSub>();
            }
        }
        //rb.AddForce(Vector3.down * 30, ForceMode.Impulse);
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
        //Debug.Log("Trigger ON: " + gameObject.name);
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
        //Debug.Log("Trigger OFF: " + gameObject.name);
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

    public void HandleCollision(GameObject source, Collision collision) {
        List<Vector3> points = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < collision.contactCount; i++) {
            ContactPoint point = collision.GetContact(i);
            points.Add(point.point);
            normals.Add(point.normal);
            //Debug.DrawLine(source.transform.position, point, Color.green, 2f);
        }
        Vector3 contactPointOnOther = Vector3Average(points);
        Vector3 contactNormal = Vector3Average(normals);
        Vector3 distanceToContactPoint = contactPointOnOther - source.transform.position;

        int layer = source.layer;
        source.layer = 7;
        int layerMask = 1 << 7;

        RaycastHit hitInfo;
        Physics.Raycast(source.transform.position + distanceToContactPoint * 5, -distanceToContactPoint.normalized, out hitInfo, distanceToContactPoint.magnitude * 5, layerMask);
        
        Vector3 interpenetration = contactPointOnOther - hitInfo.point;

        source.layer = layer;

        /*

        GameObject contactPointGO = new GameObject();
        contactPointGO.transform.position = contactPointOnOther;
        contactPointGO.transform.parent = source.transform;
        source.transform.rotation = Quaternion.identity;
        Vector3 rotatedContactPoint = contactPointGO.transform.position;
        Destroy(contactPointGO);
*/

    




        //Vector3 interpenetration = ScaleVector(distanceToContactPoint.normalized, source.transform.lossyScale / 2f);
        //Debug.DrawLine(source.transform.position, contactPointOnOther, Color.blue, 2f);
        //Debug.DrawLine(source.transform.position, interpenetration, Color.red, 2f);


        if (rb.velocity.magnitude <= VELOCITY_THRESHOLD) {
            rb.transform.position += Vector3.Project(interpenetration, contactNormal) + (contactNormal * INTERPENETRATION_BUFFER);
            rb.velocity = Vector3.zero;
        } else {
            rb.transform.position += interpenetration + (contactNormal * INTERPENETRATION_BUFFER);
            Vector3 projection = Vector3.Project(-rb.velocity, contactNormal);
            rb.AddForce((projection) * (1 + Restitution), ForceMode.Impulse);
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
