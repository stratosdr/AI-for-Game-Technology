using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxHealth = 10;
    public int currentHealth = 10;  // Player's health
    public HealthBar healthBar; // Reference to HealthBar script

    private int startingHealth;
    private Vector3 startingPosition;
    private float speed;
    private bool isGrounded;
    private bool isDead = false;

    private LevelManager levelManager;
    private AnalyticsManager analyticsManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();

        speed = walkSpeed;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        startingHealth = currentHealth;
        startingPosition = transform.position;

        levelManager = FindObjectOfType<LevelManager>();
        analyticsManager = FindObjectOfType<AnalyticsManager>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
<<<<<<< Updated upstream
        if (analyticsManager == null)
        {
            Debug.LogError("AnalyticsManager not found in the scene!");
=======

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(currentHealth);
        }
        else
        {
            Debug.LogError("HealthBar is not assigned in the Inspector!");
>>>>>>> Stashed changes
        }
    }

    void Update()
    {
        if (isDead) return;

        if (currentHealth <= 0)
        {
            isDead = true;
            analyticsManager.RecordDeath();
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

<<<<<<< Updated upstream
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = runSpeed;
            analyticsManager.RecordWalking();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = walkSpeed;
            analyticsManager.RecordSprinting();
        }
=======
        bool isMoving = moveDirection.magnitude > 0;
        animator.SetBool("isMoving", isMoving);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;
        animator.SetBool("isRunning", isRunning);

        // Adjust speed based on running state
        speed = isRunning ? runSpeed : walkSpeed;
>>>>>>> Stashed changes

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
        analyticsManager.RecordCollision();
        isGrounded = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            levelManager?.ShowYouDiedScreen();
        }
    }

    public void ResetPlayer()
    {
        transform.position = startingPosition;
        currentHealth = startingHealth;
        isDead = false;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(currentHealth);
        }
        Debug.Log("Player reset to starting state");
    }
}
