using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAIManager : MonoBehaviour
{
    private List<BirdAI> Birds = new List<BirdAI>();
    private List<GameObject> Targetlist = new List<GameObject>();
    private int targetIndex = 0;

    private void Start()
    {
        targetIndex = Random.Range(0, Targetlist.Count);
        foreach (GameObject bird in GameObject.FindGameObjectsWithTag("Bird"))
        {
            Birds.Add(bird.GetComponent<BirdAI>());
        }
    }
    // Update is called once per frame
    void Update()
    {
        print(Targetlist.Count);
       
        foreach (BirdAI bird in Birds)
        {
            if (Targetlist.Count == 0)
                bird.ResetPosition();

            else
            {
                if (  targetIndex > Targetlist.Count-1 || Vector3.Distance( bird.transform.position, Targetlist[targetIndex].transform.position) < 3f)
                    targetIndex = Random.Range(0, Targetlist.Count);
                bird.SetDestination(Targetlist[targetIndex].transform.position);
                print(Targetlist[targetIndex]);
            }

        }


    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AI")
        {
            Targetlist.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "AI")
        {
            Targetlist.Remove(other.gameObject);
        }
    }
}
