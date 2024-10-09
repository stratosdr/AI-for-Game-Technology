using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyMovement : MonoBehaviour
{
    public float wanderSpeed = 2f;           
    public float chaseSpeed = 4f;            
    public float detectionRadius = 5f;      
    public float minWanderTime = 2f;         
    public float maxWanderTime = 5f;         
    public bool drawRadius = true;
    public float knockbackForce = 5f;
    public float pauseDuration = 2f;
    private bool isPaused = false;
    

    private Transform player;
    private Vector3 wanderDirection;
    private float wanderTimer;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; 
        rb = GetComponent<Rigidbody>();  
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
        wanderDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;

        wanderTimer = Random.Range(minWanderTime, maxWanderTime);
    }

    void ChasePlayer(Transform targetPlayer)
    {
        //Move towards closest player (as there are currently 2 in the game)
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Player")) {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();
            if (p != null) {
                p.health--;
                Debug.Log("HEALTH DECREASE TO " + p.health);
                knockback(p, collision);
                StartCoroutine(PauseBehavior());
            }
        }
    }

        // Coroutine to pause the enemy for a few seconds
    IEnumerator PauseBehavior()
    {
        isPaused = true;  // Stop enemy movement

        yield return new WaitForSeconds(pauseDuration);  // Wait for the pause duration

        isPaused = false;  // Resume enemy movement
    }

    void knockback(PlayerController p, Collision collision) {
        Rigidbody playerRb = p.GetComponent<Rigidbody>();
        if (playerRb != null) {
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.VelocityChange);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the detection radius in the editor
        if (drawRadius) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}