using UnityEngine;
using TMPro;


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
    private Vector3 moveDirection;

    [Header("Player Score UI")]
    public TextMeshProUGUI playerScoreText; // Reference to the UI text that displays the player's score
    private int heartsCollected = 0; // Variable to keep track of the number of hearts collected

    void Start()
    {
        // Get the Rigidbody component attached to the same object
        rb = GetComponent<Rigidbody>();
        // Get the Animator component attached to the same object
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // ANIMATION
        bool walking = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        anim.SetBool("isWalking", walking);

        // INPUT
        // "Horizontal" corresponds to keys A/D or Left/Right arrow keys 
        inputX = Input.GetAxisRaw("Horizontal");
        
        // "Vertical" corresponds to keys W/S or Up/Down arrow keys 
        inputZ = Input.GetAxisRaw("Vertical");

        // MOVEMENT
        Vector3 moveDirection  = new Vector3(inputX, 0f, inputZ).normalized;
        // If the player is moving, we rotate the player to face the direction of movement and translate the player in that direction
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * 12f);
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // If the player falls off ground
        if (transform.position.y < -5f)
        {
            // Teleport back to the center 
            transform.position = new Vector3(0f, 2f, 0f);
            // Setting fall speed to zero
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }   

    void FixedUpdate()
    {
    
        // calculate the direction based on where the world is facing (X and Z axes)
        Vector3 moveDirection = new Vector3(inputX, 0f, inputZ).normalized;

        // apply the movement to the Rigidbody's velocity, maintaining gravity (rb.velocity.y)
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with a heart
        if (other.CompareTag("Heart"))
        {
            heartsCollected++;
            GameManager.Instance.PlayHeartSound();

            // Update the UI text to show the new score
            if (playerScoreText != null)
            {
                playerScoreText.text = "Hearts: " + heartsCollected;
                
            }
            GameManager.Instance.CheckWinCondition(heartsCollected, 0); // Check win condition for the player
            
            Destroy(other.gameObject);
        }
    }
}
