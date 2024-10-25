using UnityEngine;

public class ArrowFollow : MonoBehaviour
{
    public Transform player;   // Reference to the player
    public Transform endPoint; // Reference to the endpoint
    public float heightOffset = 2f; // Height above the player

    void Update()
    {
        if (player != null && endPoint != null)
        {
            // Keep the arrow above the player
            Vector3 playerPosition = player.position;
            playerPosition.y += heightOffset;  // Set the height offset above the player
            transform.position = playerPosition;

            // Calculate the direction vector from the player to the endpoint
            Vector3 direction = endPoint.position - playerPosition;

            // Debugging the calculated direction
            Debug.Log("Player Position: " + player.position);
            Debug.Log("EndPoint Position: " + endPoint.position);
            Debug.Log("Direction Vector: " + direction);

            // Zero out Y-axis to make sure the arrow stays horizontal
            direction.y = 0;

            // If the direction is valid, point the arrow towards the endpoint
            if (direction.sqrMagnitude > 0.01f)  // Prevent small jittering at close distances
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth transition
            }
        }
        else
        {
            // If the player or endpoint is missing, attempt to find them dynamically
            player = GameObject.FindWithTag("Player")?.transform;
            endPoint = GameObject.FindWithTag("Finish")?.transform;

            if (player == null || endPoint == null)
            {
                Debug.LogError("Player or EndPoint not assigned!");
            }
        }
    }
}
