using System;
using UnityEngine;

public class TargetPicker : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);

    [SerializeField]
    private Transform targetMarker;

    public static event Action<Vector3> targetReceiver;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //detect left mouse click and pick position to send to the receivers
        if (Input.GetMouseButtonDown(0))
        {
            if (targetReceiver != null)
            {
                Ray r = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit))
                {
                    targetMarker.position = hit.point + offset;
                    targetReceiver(targetMarker.position);
                }
            }
        }
    }
}
