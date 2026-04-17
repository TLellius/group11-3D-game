using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;
    
    public float speed = 5.0f;
    //public float rotateSpeed = 100.0f;
    public float jumpHeight = 1f;

    public float gravity = -9.81f;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask acidMask;
    public LayerMask jumpPadMask;
    public LayerMask enemyMask;

    Vector3 velocity;
    public bool isGrounded;
    public bool isOnJumpPad;
    public bool isOnAcid;
    public bool contactEnemy;
    public float acidCooldown = 1f; //1 second
    public float lastAcid = 0f;
    private PlayerHealth playerHealth;

    private Vector3 knockbackVelocity;
    public float knockbackDuration = 0.5f;
    private float knockbackTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerHealth = GetComponent<PlayerHealth>();
       if (playerHealth == null)
       {
            Debug.LogError("PlayerHealth not found on this GameObject!"); 
       }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isOnAcid = Physics.CheckSphere(groundCheck.position, groundDistance, acidMask);
        isOnJumpPad = Physics.CheckSphere(groundCheck.position, groundDistance, jumpPadMask);

        if (knockbackTimer <= 0f)
        {
            // reset downward velocity when grounded
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            if (isOnAcid && Time.time - lastAcid >= acidCooldown)
            {
                // acid damage
                playerHealth.TakeDamage(10f);
                velocity.y = Mathf.Sqrt(jumpHeight * -5f * gravity);
                lastAcid = Time.time;
            }
            if (isOnJumpPad)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -5f * gravity);
            }

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }
        else
        {
            // knockback
            velocity = knockbackVelocity;
            knockbackTimer -= Time.deltaTime;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
            if (knockbackTimer <= 0f)
            {
                velocity -= knockbackVelocity;
            }
        }
        // apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public void ApplyKnockback(Vector3 sourcePosition, float strength)
    {
        Vector3 dir = (transform.position - sourcePosition).normalized;
        knockbackVelocity = dir * strength;
        knockbackTimer = knockbackDuration;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit by enemy");
            ApplyKnockback(other.transform.position, 40f);

            playerHealth.TakeDamage(10f);
        }
    }
}
