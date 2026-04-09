using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 6f;
    [SerializeField]
    public float jumpForce = 10f;
    [SerializeField] 
    private float gravityStrength = 20f;
    [SerializeField] 
    private float fallMultiplier = 2.5f;

    private Rigidbody2D rb;
    private PlayerControls controls;

    private Vector2 moveInput;
    private Vector2 gravityInput;
    private bool isGrounded;

    private Vector2 startPosition;
    private Vector2 gravityDirection = Vector2.down;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => JumpStart();

        controls.Player.GravityShift.performed += ctx => gravityInput = ctx.ReadValue<Vector2>();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        SetGravity(Vector2.down);
    }

    void Update()
    {
        HandleGravity();
        Movement();
        HandleJump();
    }

    void Movement()
    {
        Vector2 right = new Vector2(-gravityDirection.y, gravityDirection.x);

        // Flips the controls if the gravity goes up
        if (gravityDirection == Vector2.up)
        {
            right *= -1f;
        }

        float input = moveInput.x;

        Vector2 velocity = rb.linearVelocity;
        Vector2 gravityVelocity = Vector2.Dot(velocity, gravityDirection) * gravityDirection;

        velocity = right * (input * moveSpeed);

        rb.linearVelocity = velocity + gravityVelocity;
    }

    void JumpStart()
    {
        if (isGrounded)
        {
            rb.linearVelocity += -gravityDirection * jumpForce;
        }
    }

    void HandleJump()
    {
        if (Vector2.Dot(rb.linearVelocity, gravityDirection) > 0)
        {
            rb.linearVelocity += gravityDirection * gravityStrength * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    void HandleGravity()
    {
        if (gravityInput == Vector2.zero) return;

        SetGravity(gravityInput.normalized);
    }

    void SetGravity(Vector2 dir)
    {
        gravityDirection = dir;
        Physics2D.gravity = gravityDirection * gravityStrength;

        transform.up = -gravityDirection;
    }


    void Respawn()
    {
        transform.position = startPosition;
        SetGravity(Vector2.down);
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        if (collision.gameObject.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

