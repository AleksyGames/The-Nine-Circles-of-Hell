using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player; // przypisz gracza (lub zostanie znaleziony po tagu "Player")

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Detection & Chase")]
    public float detectionRadius = 10f;
    public float loseSightRadius = 15f; // dystans, po którym przeciwnik wraca do patrolu
    public float chaseSpeed = 4f;

    [Header("Attack")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private float waitTimer;
    private readonly float lastAttackTime;
    private bool isChasing;

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
            ChasePlayer(distanceToPlayer);
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

    private void ChasePlayer(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;

        if (distanceToPlayer <= attackRange)
        {
            agent.ResetPath();
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            //TryAttack();
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    //private void TryAttack()
    //{
        //if (Time.time - lastAttackTime < attackCooldown) return;
        //lastAttackTime = Time.time;

        // tu mo¿esz odpaliæ animacjê ataku
        // Animator anim = GetComponent<Animator>();
        // if (anim) anim.SetTrigger("Attack");

        // przyk³adowe zadanie obra¿eñ
        //PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        //if (playerHealth != null)
        //{
           // playerHealth.TakeDamage(attackDamage);
        //}
        //else
        //{
           // player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        //}
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}
