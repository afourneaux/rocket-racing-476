using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullseyePositionPicker : MonoBehaviour
{
    [SerializeField]
    private Bullseye bullseye;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (bullseye.HasCollidedWith(hitInfo.point))
                {
                    Debug.Log("Position: " + hitInfo.point + " Score: " + bullseye.GetScore(hitInfo.point));
                }
            }
        }
    }
}
