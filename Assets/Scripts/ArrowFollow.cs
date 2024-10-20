using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFollow : MonoBehaviour
{
    public Transform player;       // Reference to the player
    public Transform endPoint;     // Reference to the endpoint
    public float heightOffset = 2f; // Height above the player

    void Update()
    {
        if (player != null && endPoint != null)
        {
            // Keep the arrow above the player
            Vector3 playerPosition = player.position;
            playerPosition.y += heightOffset;  // Set the height offset above the player
            transform.position = playerPosition;

            // Point the arrow towards the endpoint
            Vector3 direction = endPoint.position - transform.position;
            direction.y = 0;  // Keep the arrow horizontal
            transform.rotation = Quaternion.LookRotation(direction);
        } else {
            GameObject p = GameObject.FindWithTag("Player");
            player = p.transform;
            GameObject e = GameObject.FindWithTag("Finish");
            endPoint = e.transform;

            //GameObject end = GameObject.FindWithTag("Finish");
        }
    }
}
