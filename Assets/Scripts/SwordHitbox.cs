using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private SwordAttack sword;
    private HashSet<EnemyHP> enemiesHit = new HashSet<EnemyHP>();

    void Start()
    {
        sword = GetComponentInParent<SwordAttack>();
    }

    void OnTriggerStay(Collider other)
    {
        if (sword == null || !sword.isAttacking) return;
        if (!other.CompareTag("Enemy")) return;

        EnemyHP enemy = other.GetComponent<EnemyHP>();
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
