using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyBehavior : MonoBehaviour
{
    public float shootingRadius = 10f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootingInterval = 2f;
    private AnalyticsManager analyticsManager;

    private Transform player;
    private bool isShooting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        StartCoroutine(ShootAtPlayer());
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            isShooting = distanceToPlayer <= shootingRadius;

            if (isShooting)
            {
                FacePlayer();
            }
        }
    }

    private void FacePlayer()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Apply a 180-degree rotation offset if needed
        targetRotation *= Quaternion.Euler(0, 180, 0);

        // Smoothly rotate the cannon to face the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }


    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            if (isShooting && player != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab, transform.position + direction, Quaternion.identity);
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

                if (projectileRb != null)
                {
                    projectileRb.velocity = direction * projectileSpeed;
                }

                yield return new WaitForSeconds(shootingInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRadius);
    }
}
