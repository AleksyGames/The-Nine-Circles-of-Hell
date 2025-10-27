using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttack;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange && Time.time - lastAttack >= attackCooldown)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            lastAttack = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
