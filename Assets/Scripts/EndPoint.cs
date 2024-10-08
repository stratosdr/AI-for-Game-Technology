using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
{
    if (collider.CompareTag("Player"))
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


    }
}
}
