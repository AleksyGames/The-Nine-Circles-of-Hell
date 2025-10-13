using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControler : MonoBehaviour
{
    public int damage = 10;
    private bool canDamage = false;

    public void EnableHitbox() { canDamage = true; }
    public void DisableHitbox() { canDamage = false; }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Enemy"))
        {
            
        }
    }
}
