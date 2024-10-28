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
            Vector3 desiredPosition = player.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
