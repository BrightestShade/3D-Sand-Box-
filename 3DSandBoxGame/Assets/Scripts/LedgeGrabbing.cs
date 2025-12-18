using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("Referances")]
    public PlayerMovementAdvanced pm;
    public Transform orientation;
    public Transform cam;
    private Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;

    public float minTimeOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce; 
 
    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsledge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("Exiting")] 

    public float DistanceToLedge { get; private set; }

    private void Update()
    {
        LedgeDetection();
        SubStateMachine(); 

    }

    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0; 

        // SubState 1 - Holding onto ledge 
        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();
        }
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatIsledge);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return; 

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold(); 
    }

    private void EnterLedgeHold()
    {
        holding = true;

        pm.unlimited = true;
        pm.restricted = true; 

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero; 
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        // Move player towards ledge
        if(DistanceToLedge >  1f)
        {
            if(rb.linearVelocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime); 
        }

        // Hold onto ledge 
        else
        {
            if (!pm.freeze) pm.freeze = true;
            if (pm.unlimited) pm.unlimited = false; 
        }

        // Exiting if something goes wrong 
        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold(); 
    }

    private void ExitLedgeHold()
    {
        holding = false;
        timeOnLedge = 0f; 

        pm.restricted = false;
        pm.freeze = false;

        rb.useGravity = true;

        StopAllCoroutines();  
        Invoke(nameof(resetLastledge), 1f);
    }

    private void resetLastledge()
    {
        lastLedge = null;
    }
}
