using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPicker : MonoBehaviour
{
    [SerializeField]
    private BirdAI bird;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
            { 
                Debug.Log("New target Sent: " + hitInfo.point);
                bird.SetDestination(hitInfo.point);
            } 
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Resetting");
            bird.ResetPosition();
        }
    }
}
