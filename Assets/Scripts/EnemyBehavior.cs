using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRadius = 5f;
    public float safeRadius = 3f;  
    public float jumpForce = 10f;  
    public float jumpHeight = 5f; 
    public float duckingHeightMultiplier = 0.5f; 
    public float chargingDuration = 1.0f;  
    public float knockbackForce = 5f;
    public float jumpCooldownDuration = 5.0f;  

    private Vector3 wanderDirection;
    private float wanderTimer;
    private bool isPaused = false;
    private Transform player;
    private Rigidbody rb;

    private bool isJumping = false;
    private bool isCharging = false; 
    private bool isOnCooldown = false;  
    private Vector3 savedPlayerPosition;  
    private Vector3 originalScale; 

    // Reference to LevelManager
    private LevelManager levelManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // Find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
        originalScale = transform.localScale;

        SetNewWanderDirection();
    }

    void Update()
    {
        if (!isPaused && !isJumping && !isCharging && !isOnCooldown)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform closestPlayer = FindClosestPlayer(players);

            if (closestPlayer != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, closestPlayer.position);

                if (distanceToPlayer < detectionRadius)
                {
                    if (distanceToPlayer > safeRadius)
                    {
                        ApproachPlayer(closestPlayer);
                    }
                    else
                    {
                        savedPlayerPosition = closestPlayer.position;
                        StartCharging();
                    }
                }
                else
                {
                    Wander();
                }
            }
        }
    }

    // Method to handle enemy collision with player
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterMovement playerMovement = collision.gameObject.GetComponent<CharacterMovement>();
            if (playerMovement != null)
            {
                /*playerMovement.currentHealth--;  // Reduce player's health by 1
                Debug.Log("Player Health: " + playerMovement.currentHealth);

                // Call LevelManager to show the game over screen if the player dies
                */
                playerMovement.TakeDamage(1);
                if (playerMovement.currentHealth <= 0 && levelManager != null)
                {
                    levelManager.ShowYouDiedScreen();  // Directly call ShowYouDiedScreen instead of using SendMessage
                }

                Destroy(gameObject);

            }
        }
    }

    void Wander()
    {
        rb.MovePosition(rb.position + wanderDirection * wanderSpeed * Time.deltaTime);
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            SetNewWanderDirection();
        }
    }

    void SetNewWanderDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        wanderDirection = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)).normalized;
        wanderTimer = Random.Range(2f, 5f);
    }

    void ApproachPlayer(Transform targetPlayer)
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.deltaTime);
    }

    // Step 1: Start charging (ducking) before jumping
    void StartCharging()
    {
        isCharging = true;

        transform.localScale = new Vector3(originalScale.x, originalScale.y * duckingHeightMultiplier, originalScale.z);

        Invoke(nameof(JumpAfterCharge), chargingDuration); 
    }

    // Step 2: After charging, jump towards the saved location
    void JumpAfterCharge()
    {
        transform.localScale = originalScale;

        StartJump(savedPlayerPosition);
    }

    void StartJump(Vector3 targetPosition)
    {
        isCharging = false;  
        isJumping = true;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        Vector3 horizontalVelocity = directionToTarget * jumpForce;

        float verticalVelocity = Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude);

        Vector3 jumpVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        rb.velocity = jumpVelocity;

        // Begin cooldown period after jump starts
        StartCoroutine(HandleJumpCooldown());
    }

    // Step 3: Handle jump cooldown period
    private IEnumerator HandleJumpCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(jumpCooldownDuration);

        isJumping = false;
        isOnCooldown = false;
    }


    Transform FindClosestPlayer(GameObject[] players)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject player in players)
        {
            Vector3 directionToPlayer = player.transform.position - currentPosition;
            float dSqrToPlayer = directionToPlayer.sqrMagnitude;

            if (dSqrToPlayer < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToPlayer;
                bestTarget = player.transform;
            }
        }

        return bestTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, safeRadius);
    }
}
