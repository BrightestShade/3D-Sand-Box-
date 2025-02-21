using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;

public class GrapplingMovement : MonoBehaviour
{

    // References
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask isGrappleable;
    public LineRenderer lr;
    
    // Grappling 
    public float maxGrappleDistance;
    public float grappleDelayTime;

    public float overshootYAxis;

    private Vector3 grapplePoint;

    //Cooldown
    public float grapplingCooldown;
    private float grapplingCooldownTime;

    // Input
    public KeyCode grappleKey = KeyCode.Mouse1; // assigns the left mouse button to shoot grapple 
    public bool grappling;


    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();

    }


    private void LateUpdate()
    {
        if(grappling) 
        {
          lr.SetPosition(0, gunTip.position);
        }
    }


private void StartGrapple()
    {
        if (grapplingCooldownTime > 0)
        {
            return;
        }
        grappling = true;
        RaycastHit hit;

        pm.freeze = true;

        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, isGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        lr.enabled = true; 
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        pm.freeze = false;
        grappling = false;
        grapplingCooldownTime = grapplingCooldown;

        lr.enabled = false;
    }

    private void Update()
    {
         if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }
         if(grapplingCooldownTime > 0)
        {
            grapplingCooldownTime -= Time.deltaTime;
        }
    }
}
