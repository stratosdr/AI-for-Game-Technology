using UnityEngine;

public class EndLevel : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ShowGameEndScreen();
        }
    }

    void ShowGameEndScreen() {
        Debug.Log("Player reached the end! Game over.");
        // GameManager.Instance.ShowEndScreen();
    }
}