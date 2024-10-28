using UnityEngine;
using UnityEngine.UI;  // For Image handling

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    public int maxHealth = 10;
    public int currentHealth = 10;  // Player's current health
    public float maxstamina = 100;  
    public float stamina = 100;  
    public HealthBar healthBar;  // Reference to HealthBar script
    public StaminaBar staminabar; 

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
        speed = walkSpeed;
        currentHealth = maxHealth;

        // Ensure HealthBar is assigned and set max health

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

    void FixedUpdate()
    {
        if(speed == runSpeed) stamina-=1;
            // Gradual stamina recovery if walking and stamina is less than max
    if (speed == walkSpeed && stamina < maxstamina) 
    {
        float recoveryRatio = (float)stamina / maxstamina; // Ratio between 0 and 1
        float recoveryRate = Mathf.SmoothStep(0.2f, 2, recoveryRatio); // Ease function
        stamina += recoveryRate; // Apply recovery based on smoothstep
    }

    // Clamp stamina to max value
    stamina = Mathf.Clamp(stamina, 0, maxstamina);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(stamina > 5){
                speed = runSpeed;
                analyticsManager.RecordSprinting();
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || stamina < 1)
        {
            speed = walkSpeed;
            analyticsManager.RecordWalking();
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

    public void ResetPlayer()
    {
        transform.position = startingPosition;
        currentHealth = maxHealth;
        isDead = false;

        Debug.Log("Player reset to starting state");
    }
}
