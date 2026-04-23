using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    private Animator animator;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    public Rigidbody rb;

    // attacking
    public float lungeDistance = 2f;
    public float lungeSpeed = 10f;
    private bool isLunging = false;
    public float lungeDuration = 4f;
    private float lungeTimer;
    private Vector3 targetDirection;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //update Enemy GameObject dest to Player gameobject
        /*if(player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }*/

        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!isLunging)
        {
            if(!playerInSight && !playerInAttackRange) Patrol();
            else if(playerInSight && !playerInAttackRange) Chase();
            else if(playerInSight && playerInAttackRange) 
            {
                Vector3 playerPos = player.transform.position;
                playerPos.y = 0f;
                transform.LookAt(playerPos);
                Attack();
            }
        }
    }

    void FixedUpdate()
    {
        targetDirection = player.position - transform.position;
        targetDirection = targetDirection.normalized;
        targetDirection.y = 0f;
        if (isLunging)
        {
            //navMeshAgent.SetDestination(transform.position);
            rb.MovePosition(rb.position + targetDirection * lungeSpeed * Time.fixedDeltaTime);

            lungeTimer -= Time.fixedDeltaTime;

            if (lungeTimer <= 0f)
            {
                isLunging = false;
                navMeshAgent.enabled = true;
            }
        }
    }

    void Chase()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            animator.SetTrigger("Attack");
            navMeshAgent.enabled = false;
            //navMeshAgent.SetDestination(transform.position);
        }
    }
    
    void Lunge()
    {
        //rb.rotation = Quaternion.LookRotation(targetDirection);

        //Debug.Log(targetDirection);
        //Debug.Log("Enemy: " + transform.position + " Player: " + player.position);
        navMeshAgent.enabled = false;

        //rb.linearVelocity = Vector3.zero;
        //rb.AddForce(targetDirection * lungeDistance, ForceMode.Impulse);
        //transform.LookAt(player);
        //navMeshAgent.SetDestination(player.transform.position);

        isLunging = true;
        lungeTimer = lungeDuration;
    }

    void Patrol()
    {
        if (!walkpointSet) SearchForDest();
        if (walkpointSet) navMeshAgent.SetDestination(destPoint);
        if(Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        // Checks if destPoint is in the nav mesh
        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void EnableAttack()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void DisableAttack()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*var player = other.GetComponent<PlayerMovement>();

        if(player != null)
        {
            print("HIT!");
        }*/

    }

}
