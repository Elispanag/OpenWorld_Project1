using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // Speed at which the player moves
    public float moveSpeed = 10f;

    // Reference to the Rigidbody component of the player
    private Rigidbody rb;

    // Variables for storing the input (input) from the keyboard
    private float inputX;
    private float inputZ;

    private Animator anim;

    void Start()
    {
        // Get the Rigidbody component attached to the same object
        rb = GetComponent<Rigidbody>();
        // Get the Animator component attached to the same object
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {

        bool walking = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

        anim.SetBool("isWalking", walking);
        // GET INPUT (Input) every frame
        // "Horizontal" corresponds to keys A/D or Left/Right arrow keys (-1 to 1)
        inputX = Input.GetAxisRaw("Horizontal");
        
        // "Vertical" corresponds to keys W/S or Up/Down arrow keys (-1 to 1)
        inputZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(inputX, 0f, inputZ);

        if (moveDirection != Vector3.zero)
        {
            // Rotate the player to face the direction of movement
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * 12f);
        }
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
