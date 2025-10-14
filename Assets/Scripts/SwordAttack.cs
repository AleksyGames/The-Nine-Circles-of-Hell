using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Animator animator;
    public float attackDuration = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public float damage = 10f;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool attackJustStarted = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack());
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        attackJustStarted = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }


}
