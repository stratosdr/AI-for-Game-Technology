using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController cc;
    [SerializeField] private Camera cam;
    [SerializeField] private float sensitivity = 200f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float runSpeed = 10f;

    private float xRotation = 0f;
    private float speed;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        speed = walkSpeed;
    }
    void Update()
    {
    float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

    transform.Rotate(Vector3.up * mouseX);
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    isGrounded = cc.isGrounded;
    if (isGrounded && velocity.y < 0)
    {
        velocity.y = -5f;
    }

    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    Vector3 direction = transform.forward * vertical + transform.right * horizontal;
    Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;

    cc.Move(projectedDirection * speed * Time.deltaTime);
    if (Input.GetKey(KeyCode.LeftShift))
    {
        speed = runSpeed;
    }
    else
    {
        speed = walkSpeed;
    }
    velocity.y += gravity * Time.deltaTime;
    cc.Move(velocity * Time.deltaTime);
    }


}
