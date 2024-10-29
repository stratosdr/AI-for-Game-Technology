using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFollow : MonoBehaviour
{
    public Transform player;       // Reference to the player
    public Transform endPoint;     // Reference to the endpoint
    public float heightOffset = 2f; // Height above the player
    public float arrowDistance = .4f;

    void Update()
    {
        if (player != null && endPoint != null)
        {
            Vector3 direction = endPoint.position - player.position;
            // direction.y = 0; 

            Vector3 offset = direction.normalized * arrowDistance;
            
            // Place the arrow relative to the player position
            transform.position = player.position + offset;

            transform.rotation = Quaternion.LookRotation(direction);
            transform.rotation *= Quaternion.Euler(0,90,0);
        } else {
            GameObject p = GameObject.FindWithTag("Player");
            player = p.transform;
            GameObject e = GameObject.FindWithTag("Finish");
            endPoint = e.transform;

            //GameObject end = GameObject.FindWithTag("Finish");
        }
    }
}
