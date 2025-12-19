using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;


    public float wallrunSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    bool readyToJump = true; // Initialize to true

    [Header("Double Jump")]
    public int maxJumps = 2;
    public int jumpsRemaining;

    [Header("Stamina")]
    public Image StaminaBar;
    public float Stamina, MaxStamina;

    public float ChargeRate;

    private Coroutine recharge;

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

    public float dashSpeed = 50f;
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

    [Header("Wallrun Stamina")]
    public float wallrunStaminaCost = 5f;


    public MovementState state;

    
    public enum MovementState
    {
        walking,
        sprinting,
        airborne,
        wallrunning,
        crouching,
        sliding,
        dashing,
    }
    public bool sliding;
    public bool dashing;
    public bool wallrunning;

    

    public float SprintCost;
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
        HandleStamina();

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.red);

        if (grounded)
        {
            jumpsRemaining = maxJumps;
        }

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        if (freeze)
        {
       
            rb.linearVelocity = Vector3.zero;

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
        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0 && readyToJump)
        {
            readyToJump = false;
            Jump();
            jumpsRemaining--;
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch input
        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            Debug.Log("Crouching");
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
        // if wallrunning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }





        if (dashing)
        {
            state = MovementState.dashing;
            moveSpeed = dashSpeed;
        }


        // If sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;

            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }


        }

        else if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }


        // If Sprinting
        else if (grounded && Input.GetKey(sprintKey) && Stamina > 0)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            Stamina -= SprintCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;

            StaminaBar.fillAmount = Stamina / MaxStamina;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
        else if (grounded && Input.GetKey(sprintKey) && Stamina <= 0)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }


        // If Walking
        else if (grounded)
        {
          //  Debug.Log("Walking");
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        
        // If Airboirne
        else 
        {
           // Debug.Log("Airborne");
            state = MovementState.airborne;
        }


        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }


        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void HandleStamina()
    {
        // Drain stamina if sprinting or wallrunning
        if ((state == MovementState.sprinting && Stamina > 0) || (wallrunning && Stamina > 0))
        {
            float drain = wallrunning ? wallrunStaminaCost * Time.deltaTime : SprintCost * Time.deltaTime;
            Stamina -= drain;
            Stamina = Mathf.Max(Stamina, 0f);
            StaminaBar.fillAmount = Stamina / MaxStamina;

            // Stop any ongoing recharge while draining
            if (recharge != null)
            {
                StopCoroutine(recharge);
                recharge = null;
            }
        }
        else
        {
            // If not already recharging, start the coroutine
            if (Stamina < MaxStamina && recharge == null)
            {
                recharge = StartCoroutine(RechargeStamina());
            }
        }
    }


    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;
        
        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (sliding && OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * slideSpeed * 20f, ForceMode.Force);

                // Apply a downward force to keep the player on the slope
                rb.AddForce(Vector3.down * 40f, ForceMode.Force);
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

        rb.useGravity = !OnSlope();
            
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;
        if (dashing) return;
        // Slope speed limiting
        if(rb.linearVelocity.magnitude > desiredMoveSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * desiredMoveSpeed;
        }
        //Ground & Air speed limiting
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > desiredMoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * desiredMoveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
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
        rb.linearVelocity = velocityToSet;
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

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;

        }

        return false;
    }
    
   public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }




    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f); // Optional delay before regen

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate * 0.1f;
            Stamina = Mathf.Min(Stamina, MaxStamina);
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(0.1f);
        }

        // Done recharging
        recharge = null;
    }


























}
