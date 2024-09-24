using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;  // Reference to the player's transform
    public Vector3 offset;     // Offset from the player's position
    public float smoothSpeed = 0.125f;  // Smoothing factor for the camera movement

    // Method to assign the player (called when the player is instantiated)
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Desired position: offset from the player's position (top-down view)
            Vector3 desiredPosition = player.position + offset;

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera position
            transform.position = smoothedPosition;

            // Rotate the camera for a top-down view with a slight tilt
            transform.rotation = Quaternion.Euler(45f, 0f, 0f); // 45-degree tilt for top-down view
        }
    }
}
