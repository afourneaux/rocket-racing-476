using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    float PowerUpTime = 4.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Racer"))
        {
            StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider racer)
    {
        Rigidbody rb = racer.GetComponent<Rigidbody>();
        rb.velocity *= 1.25f;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(PowerUpTime);
        rb.velocity /= 1.25f;
        Destroy(this.gameObject);
    }
}
