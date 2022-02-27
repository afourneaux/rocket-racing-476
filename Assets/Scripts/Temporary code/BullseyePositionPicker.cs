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
                Debug.Log("Position: " + hitInfo.point + " Score: " + bullseye.GetScore(hitInfo.point));
            }
        }
    }

    // Checks to make sure an object properly collided with the bullseye since 
    //the bullseye uses a box collider even though it is a circular shape
    public bool HasCollidedWithBullseye(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) < transform.localScale.x;
    }
}
