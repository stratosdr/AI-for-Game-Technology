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
    public float pushForce = 5f;

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

        if (closestPlayer != null)
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
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                //For now it will instantly restart the level on contact, health needs to be implemented onto the player
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                
                
                
                //Vector3 pushDirection = collision.transform.position - transform.position;
                //pushDirection.y = 0;  // Remove any vertical influence
                //pushDirection.Normalize();
                
                
                //NEEDS TO BE FIXED
                //playerController.ApplyPush(pushDirection, pushForce);
            }
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