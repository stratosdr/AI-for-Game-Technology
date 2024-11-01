using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform player;
    public float zoomSpeed = 5f;
    public float targetDistance = 1f;
    public float targetFieldOfView = 20f;
    private Camera mainCamera;
    private bool isZooming = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isZooming)
        {
            Vector3 targetPosition = player.position - player.forward * targetDistance + Vector3.up * 1.5f;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * zoomSpeed);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }
    }

    public void StartZoom()
    {
        isZooming = true;
    }
}
