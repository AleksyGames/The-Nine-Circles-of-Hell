using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator; // Animator postaci
    public float attackCooldown = 0.5f; // minimalny odstêp miêdzy ciosami
    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // LPM – klikniêcie
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
