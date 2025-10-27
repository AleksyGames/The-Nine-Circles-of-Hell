using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [HideInInspector] public NavMeshAgent Agent;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Detection & Chase")]
    public float detectionRadius = 10f;
    public float loseSightRadius = 15f;
    public float chaseSpeed = 4f;
    public float rotationSpeed = 8f; // prêdkoœæ obracania w stronê gracza

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private float waitTimer;
    private bool isChasing;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Brak komponentu NavMeshAgent! Dodaj go do przeciwnika.");
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        agent.speed = patrolSpeed;
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --- LOGIKA GONIENIA ---
        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > loseSightRadius)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.4f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                waitTimer = 0f;
            }
        }
    }

    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        // Obracanie przeciwnika w stronê gracza
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // nie obracaj po osi Y (¿eby nie patrzy³ w dó³ lub górê)

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}
