using System.Collections;
using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
    public float radius = 3f;
    public float detonationSpeed = 2f;
    public float knockbackConcusionTime = 1f;
    public float explosionPower = 10f;
    public GameObject radiusVisualizer; // Reference to the radius display GameObject

    private static int idle = 0;
    private static int detonating = 1;
    private static int detonated = 2;
    private int state = idle;

    private float detonationTimer = 0f;

    private AnalyticsManager analyticsManager;

    public AudioSource audioSource;
    public AudioClip fuzeAudio;


    void Start()
    {
        

        // Ensure the radius display is initially hidden
        if (radiusVisualizer != null)
        {
            radiusVisualizer.SetActive(false);
            radiusVisualizer.transform.localScale = new Vector3(radius*4, 0.0001f, radius*4);
        }
        analyticsManager = FindObjectOfType<AnalyticsManager>();
    }


    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (state == idle)
        {
            // Show radius display if player is within the radius
            if (distanceToPlayer <= (radius + 2) && radiusVisualizer != null)
            {
                radiusVisualizer.SetActive(true);
            }
            else if (radiusVisualizer != null)
            {
                radiusVisualizer.SetActive(false);
            }

            if (distanceToPlayer > radius) return;

            // Start detonation if player is within the radius
            state = detonating;
            detonationTimer = 0f;
            Debug.Log("I'm starting to detonate!");
            if(!audioSource.isPlaying) audioSource.PlayOneShot(fuzeAudio);

            analyticsManager.RecordBombIgnition();
        }
        else if (state == detonating)
        {
            detonationTimer += Time.deltaTime;

            if (detonationTimer >= detonationSpeed)
            {
                Detonate();
                state = detonated;

                Destroy(gameObject);
            }
        }
    }

    void Detonate()
    {
        GameObject player = GameObject.FindWithTag("Player");

        CharacterMovement playerScript = player.GetComponent<CharacterMovement>();
        playerScript.playBombExplosion();
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > radius) {analyticsManager.RecordBombMissed(); return;}

        // Hide the radius display just before detonating
        if (radiusVisualizer != null)
        {
            radiusVisualizer.SetActive(false);
        }

        playerScript.TakeDamage(1);
        playerScript.KnockBack(transform.position, explosionPower, knockbackConcusionTime);
        Debug.Log("Bomb detonated!");
        analyticsManager.RecordBombDamage(1);

        Renderer renderer  = GetComponent<Renderer>();
        renderer.enabled = false;

        Debug.Log(renderer.enabled);

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius+2);
    }
}
