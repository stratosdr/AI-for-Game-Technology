using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController cc;           // The character controller attached to the player
    [SerializeField] private Camera cam;     // The camera that will rotate
    [SerializeField] private float sensitivity = 200f;  // Sensitivity for mouse movement
    [SerializeField] private float walkSpeed = 5f;      // Walking speed
    [SerializeField] private float runSpeed = 10f;      // Running speed

    private float xRotation = 0f;            // To store the current X-axis rotation
    private float speed;                     // Current movement speed

    void Start()
    {
        cc = GetComponent<CharacterController>();
        speed = walkSpeed;  // Set the initial speed to walking
    }

    void Update()
    {
        // Get mouse movement for camera rotation
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate the player (Y-axis rotation)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera (X-axis rotation)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Clamp rotation to prevent over-rotation
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // Apply rotation to the camera

        // Get input for player movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        cc.Move(direction * speed * Time.deltaTime);

        // Check if the player is running or walking
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }
}
