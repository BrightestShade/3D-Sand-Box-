using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Wallrunning : MonoBehaviour
{


    [Header("Wallrunning")]

    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;
    public float wallClimbSpeed;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    public bool upwardsRunning;
    public bool downwardsRunning;
    public KeyCode jumpKey = KeyCode.Space;


    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public CameraMovement cam;
    private PlayerMovement pm;
    private Rigidbody rb;
    

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // Exiting wallrun
        if (exitingWall)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        // Wallrunning
        else if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
            {
                StartWallRun();
            }

            // wallRunTimer
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && pm.wallrunning) 
            {
                exitingWall = true;
               exitWallTimer = exitWallTime;
            }


            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }

        // Stop wallrun if needed
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }


    private void StartWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);

        // Reset double jumps when starting wallrun
        pm.jumpsRemaining = pm.maxJumps;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward). magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // Forward Force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);


        // Upwards/downwards force
        if (upwardsRunning)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
        }

        if (downwardsRunning)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);
        }


        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Reset jump Velocity & Add force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

    }
}
