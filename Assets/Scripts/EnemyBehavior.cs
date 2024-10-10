using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRadius = 5f;
    public float knockbackForce = 5f;

    private Vector3 wanderDirection;
    private float wanderTimer;
    private bool isPaused = false;
    private Transform player;
    private Rigidbody rb;

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

        SetNewWanderDirection();
    }

    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = FindClosestPlayer(players);

        if (closestPlayer != null && !isPaused)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, closestPlayer.position);

            if (distanceToPlayer < detectionRadius)
            {
                ChasePlayer(closestPlayer);
            }
            else
            {
                Wander();
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
                playerMovement.health--;  // Reduce player's health by 1
                Debug.Log("Player Health: " + playerMovement.health);

                // Call LevelManager to show the game over screen if the player dies
                if (playerMovement.health <= 0 && levelManager != null)
                {
                    levelManager.ShowYouDiedScreen();  // Directly call ShowYouDiedScreen instead of using SendMessage
                }
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

    void ChasePlayer(Transform targetPlayer)
    {
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.deltaTime);
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
    }
}
