using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooterAttack : MonoBehaviour
{
    private Transform target;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 1f;
    public float shootDistance = 10f;  // dystans do zatrzymania i strzelania
    public float bulletSpeed = 20f;

    private float cooldown;
    private EnemyMovement movement;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= shootDistance)
        {
            // zatrzymaj ruch
            if (movement != null && movement.Agent != null)
                movement.Agent.isStopped = true;

            // strzelaj
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                Shoot();
                cooldown = 1f / fireRate;
            }
        }
        else
        {
            // wznów ruch
            if (movement != null && movement.Agent != null)
                movement.Agent.isStopped = false;
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector3 dir = (target.position - shootPoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = dir * bulletSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootDistance);
    }
}
