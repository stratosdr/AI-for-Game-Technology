using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb; // Use Rigidbody instead of CharacterController
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float pushDecayRate = 2f;

    private float speed;
    private Vector3 pushDirection = Vector3.zero;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.freezeRotation = true; // Prevent player from tipping over
        speed = walkSpeed;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Shift-to-run
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = walkSpeed;
        }

        // Apply movement relative to the player's orientation
        Vector3 movement = transform.TransformDirection(moveDirection) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z); // Retain y-axis for gravity

        // Gravity remains applied on the y-axis, so it won't interfere with horizontal speed
        if (isGrounded && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else
        {
            rb.velocity += new Vector3(0f, gravity * Time.deltaTime, 0f);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("COLLISION: " + collision.gameObject.name);
        
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // Detect if grounded (simplified ground check)
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // When exiting a collision, consider the player no longer grounded
        isGrounded = false;
    }
}