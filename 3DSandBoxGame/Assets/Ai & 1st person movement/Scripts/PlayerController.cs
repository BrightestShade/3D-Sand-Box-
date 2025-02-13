using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.8f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
    }

    void Update()
    {
        
            // Ground check
            isGrounded = characterController.isGrounded;

            // Apply gravity
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            // Get input for movement
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // Movement vector
            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            // Move player
            characterController.Move(move * moveSpeed * Time.deltaTime);

            // Jumping
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            // Apply velocity to the character
            characterController.Move(velocity * Time.deltaTime);

            // Mouse look (for turning)
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up * mouseX * turnSpeed);
            Camera.main.transform.Rotate(Vector3.left * mouseY * turnSpeed);
       
        
    }
}