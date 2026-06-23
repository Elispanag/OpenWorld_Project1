using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Speed at which the player moves
    public float moveSpeed = 7f;

    // Reference to the Rigidbody component of the player
    private Rigidbody rb;

    // Variables for storing the input (input) from the keyboard
    private float inputX;
    private float inputZ;

    void Start()
    {
        // Get the Rigidbody component attached to the same object
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // GET INPUT (Input) every frame
        // "Horizontal" corresponds to keys A/D or Left/Right arrow keys (-1 to 1)
        inputX = Input.GetAxisRaw("Horizontal");
        
        // "Vertical" corresponds to keys W/S or Up/Down arrow keys (-1 to 1)
        inputZ = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // MOVEMENT PHYSICS
        // The FixedUpdate method is used for smooth movement through Rigidbody, independent of FPS
        
        // We calculate the direction based on where the world is facing (X and Z axes)
        Vector3 moveDirection = new Vector3(inputX, 0f, inputZ).normalized;

        // We apply the movement to the Rigidbody's velocity, maintaining gravity (rb.velocity.y)
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
}
