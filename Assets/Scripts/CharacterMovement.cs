using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    public int maxHealth = 10;
    public int currentHealth = 10;  // Player's current health
    public float maxStamina = 100;  // Renamed to maxStamina
    public float stamina = 100;
    public HealthBar healthBar;  // Reference to HealthBar script
    public StaminaBar staminaBar;

    private Vector3 startingPosition;
    private float speed;
    private bool isGrounded;
    private bool isDead = false;
    private LevelManager levelManager;
    private AnalyticsManager analyticsManager;
    private bool canMove = true;  // Movement control flag
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        speed = walkSpeed;
        currentHealth = maxHealth;

        startingPosition = transform.position;
        levelManager = FindObjectOfType<LevelManager>();
        analyticsManager = FindObjectOfType<AnalyticsManager>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
        if (analyticsManager == null)
        {
            Debug.LogError("AnalyticsManager not found in the scene!");
        }
    }

    void Update()
    {
        if (isDead || !canMove) return;

        if (currentHealth <= 0)
        {
            isDead = true;
            analyticsManager.RecordDeath();
            levelManager?.ShowYouDiedScreen();  // Call the LevelManager to show YouDiedPanel
            return;
        }

        HandleMovement();
    }

    void FixedUpdate()
    {
        if (speed == runSpeed)
        {
            stamina -= 1;
        }

        // Gradual stamina recovery if walking and stamina is less than max
        if (speed == walkSpeed && stamina < maxStamina)
        {
            float recoveryRatio = stamina / maxStamina; // Ratio between 0 and 1
            float recoveryRate = Mathf.SmoothStep(0.2f, 2, recoveryRatio); // Ease function
            stamina += recoveryRate; // Apply recovery based on smoothstep
        }

        // Clamp stamina to max value
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Set isWalking based on WASD keys input
        bool isMoving = moveDirection.magnitude > 0;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift) && stamina > 5)
            {
                speed = runSpeed;
                animator.SetBool("isRunning", true);
                stamina -= Time.deltaTime * 10; // Decrease stamina when running
            }
            else
            {
                speed = walkSpeed;
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Stamina recovery if not running
        if (!Input.GetKey(KeyCode.LeftShift) && stamina < maxStamina)
        {
            stamina += Time.deltaTime * 5;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        // Apply movement to Rigidbody
        Vector3 movement = transform.TransformDirection(moveDirection) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Gravity application
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

    // Method to handle damage taken by player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth + " max health: " + maxHealth);
    }

    public void KnockBack(Vector3 origin, float knockbackForce, float knockbackConcusionTime)
    {
        Vector3 knockbackDirection = (transform.position - origin).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        StartCoroutine(KnockbackCooldown(knockbackConcusionTime));
    }

    private IEnumerator KnockbackCooldown(float knockbackConcusionTime)
    {
        canMove = false;
        yield return new WaitForSeconds(knockbackConcusionTime);
        canMove = true;
    }

    public void ResetPlayer()
    {
        transform.position = startingPosition;
        currentHealth = maxHealth;
        isDead = false;

        Debug.Log("Player reset to starting state");
    }
}
