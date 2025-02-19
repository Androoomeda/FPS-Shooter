using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;

    public int GetCurrentHealth() => currentHealth;

    public Action OnDie { get; internal set; }

    public int currentHealth;

    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            OnDie.Invoke();
    }
}