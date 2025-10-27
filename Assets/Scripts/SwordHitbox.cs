using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private SwordAttack sword;
    private HashSet<EnemyHealth> enemiesHit = new HashSet<EnemyHealth>();

    void Start()
    {
        sword = GetComponentInParent<SwordAttack>();
    }

    void OnTriggerStay(Collider other)
    {
        if (sword == null || !sword.isAttacking) return;
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        if (!enemiesHit.Contains(enemy))
        {
            enemy.TakeDamage(sword.damage);
            enemiesHit.Add(enemy);
        }
    }

    void Update()
    {
        if (sword.isAttacking && enemiesHit.Count > 0 && sword.attackJustStarted)
        {
            enemiesHit.Clear();
            sword.attackJustStarted = false;
        }
    }
}
