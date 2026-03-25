using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 5f;
    public float jumpHeight = 5f;
    public float gravity = -20f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask acidMask;
    public LayerMask enemyMask;

    public bool isGrounded;
    public bool isOnAcid;
    public bool contactEnemy;

    public float acidCooldown = 1f;
    public float lastAcid = 0f;

    public float knockbackDuration = 0.5f;
    float knockbackTimer = 0f;
    Vector3 knockbackVelocity;

    Vector3 velocity;

    private PlayerHealth playerHealth;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
            Debug.LogError("PlayerHealth not found!");
    }

    void Update()
    {
        // check if on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isOnAcid = Physics.CheckSphere(groundCheck.position, groundDistance, acidMask);

        // reset downward velocity when grounded
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // acid damage
        if (isOnAcid && Time.time - lastAcid >= acidCooldown)
        {
            playerHealth.TakeDamage(10f);
            lastAcid = Time.time;
        }

        // movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (knockbackTimer <= 0f)
        {
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            // jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // knockback
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit by enemy");
            ApplyKnockback(other.transform.position, 40f);
            playerHealth.TakeDamage(10f);
        }
    }
}