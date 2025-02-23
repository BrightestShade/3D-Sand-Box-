using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    bool readyToJump = true; // Initialize to true

   

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;
  
    public bool freeze;

    public bool activeGrapple;

    private bool enableMovementOnNextTouch;


    [Header("MovementModes")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("KeyCodes")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("SlopeMovement")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;


    public MovementState state;

    
    public enum MovementState
    {
        walking,
        sprinting,
        airborne,
        crouching,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.red);

        if (grounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        if (freeze)
        {
       
            rb.velocity = Vector3.zero;

        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump input
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch input
        if(Input.GetKeyDown(crouchKey))
        {
           transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {


        // If Walking
        if (grounded)
        {
          //  Debug.Log("Walking");
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // If Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
          //  Debug.Log("Sprinting");
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        if (Input.GetKey(crouchKey))
        {
          //  Debug.Log("Crouching");
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        
       
        // If Airboirne
        else 
        {
           // Debug.Log("Airborne");
            state = MovementState.airborne;
        }
    }


    private void MovePlayer()
    {
        if (activeGrapple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // If on slope
        if (OnSlope())
        {
            Vector3 slopeMoveDirection = GetSlopeMoveDirection();

            // Apply force only if player is not already moving upwards too fast
            if (rb.velocity.y < 12f)
            {
                rb.AddForce(slopeMoveDirection * moveSpeed * 20f, ForceMode.Force);
            }
        }
      

        // On ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // In air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Disable gravity whilst on slope (Stops player slipping down slope)
        rb.useGravity = !OnSlope();
            
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;
        
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    // Calculate the force needed to push the player to the end point of the grapple
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        if (gravity >= 0) throw new System.Exception("Gravity must be negative");

        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        if (trajectoryHeight <= 0)
        {
           
            return Vector3.zero;
        }

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        float timeToReachApex = Mathf.Sqrt(-2 * trajectoryHeight / gravity);
        float timeToReachEnd = timeToReachApex + Mathf.Sqrt(2 * Mathf.Max(0, displacementY - trajectoryHeight) / -gravity);

        if (timeToReachEnd <= 0)
        {
            
            return Vector3.zero;
        }

        Vector3 velocityXZ = displacementXZ / timeToReachEnd;

        return velocityXZ + velocityY;
    }


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<GrapplingMovement>().StopGrapple();

        }
    }


    
    private bool OnSlope()
    {

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3 .Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;


    }


    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    





























}
