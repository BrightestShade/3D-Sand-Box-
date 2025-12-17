using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("Referances")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float Climbspeed;
    public float maxClimbTime;
    private float ClimbTimer;

    private bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public float ClimbSpeed { get; private set; }

    private void Update()
    {
        WallCheck(); 
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal); 
    }

    private void StartClimbing()
    {
        climbing = true; 

        // camera fov change 
    }

    private void ClimbingMovement()
    {
        rb.angularVeloc

        // sound effect 
    }

    private void StopClimbing()
    {
        climbing = false; 

        // particle effect 
    }
}
