using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{

    public float maxHP = 100f;
    private float enemyHP;

    private void Start()
    {
        enemyHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        enemyHP -= damage;
        Debug.Log($"Enemy HP: {enemyHP}");

        if (enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
