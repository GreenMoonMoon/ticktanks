using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamagable
{
    public float health;
    public GameObject powerup;

    void OnDestroy()
    {
        if (powerup != null)
            Instantiate(powerup, transform.position, Quaternion.identity);
    }

    public void Damage()
    {
        health -= 1;
        if (health <= 0)
            Destroy(gameObject);
    }
}
