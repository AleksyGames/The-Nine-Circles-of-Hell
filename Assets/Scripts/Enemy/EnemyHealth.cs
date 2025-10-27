using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50;      // maksymalne ¿ycie przeciwnika
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Funkcja wywo³ywana przez Bullet.cs
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject); // usuwa przeciwnika
    }

}
