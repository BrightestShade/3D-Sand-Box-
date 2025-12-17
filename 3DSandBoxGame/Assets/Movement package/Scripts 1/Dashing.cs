using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform MainCamera;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;


    [Header("Dashing cooldown")]
    public float dashCoolDown;
    public float dashCoolDownTimer;

    [Header("Keycodes")]
    public KeyCode dashKey = KeyCode.Q;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(dashKey) && !pm.grounded)
        {
            Dash();
        }
        if (dashCoolDownTimer > 0)
        {
            dashCoolDownTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if (dashCoolDownTimer > 0) return;
        else dashCoolDownTimer = dashCoolDown;

        pm.dashing = true;

      Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;
        rb.AddForce(forceToApply, ForceMode.Impulse);

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
    }
}
