using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private Transform model_transform;

    public float smoothTime = 1000f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(player == null) {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
        model_transform = transform.Find("Cannon");

        StartCoroutine(ShootAtPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= shootingRadius) {
                isShooting = true;
                Vector3 direction = (player.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, smoothTime * Time.deltaTime);
                Quaternion rotation = Quaternion.Euler(-90, smoothAngle + 90, 0);
                model_transform.rotation = rotation;
            } else {
                isShooting = false;
            }
        }
    }

    private IEnumerator ShootAtPlayer() {
        while (true) {
            if (isShooting && player != null) {
                Vector3 direction = (player.position - transform.position).normalized;
                GameObject projectile = Instantiate (projectilePrefab, transform.position + direction, Quaternion.identity);
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

                if (projectileRb != null) {
                    projectileRb.velocity = direction * projectileSpeed;
                }

                //Destroy(projectile, 5f);
                yield return new WaitForSeconds(shootingInterval);
            } else {
                yield return null;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRadius);
    }
}
