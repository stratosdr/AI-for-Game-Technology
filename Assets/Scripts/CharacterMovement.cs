using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    public int health = 10;  // Player's health
    private int startingHealth;
    private Vector3 startingPosition;  // Store player's initial position
    private float speed;
    private bool isGrounded;
    private bool isDead = false;

    private LevelManager levelManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        speed = walkSpeed;

        startingHealth = health;
        startingPosition = transform.position;  // Store initial player position

        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene! Make sure there is a LevelManager GameObject.");
        }
    }

    void Update()
    {
        if (isDead) return;

        if (health <= 0)
        {
            isDead = true;
            levelManager?.ShowYouDiedScreen();  // Call the LevelManager to show YouDiedPanel
            return;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = walkSpeed;
        }

        Vector3 movement = transform.TransformDirection(moveDirection) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        if (isGrounded && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else
        {
            rb.velocity += new Vector3(0f, gravity * Time.deltaTime, 0f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // Add this method to reset player state
    public void ResetPlayer()
    {
        transform.position = startingPosition;  // Reset position
        health = startingHealth;  // Reset health
        isDead = false;  // Reset death state
        Debug.Log("Player reset to starting state");
    }
}
