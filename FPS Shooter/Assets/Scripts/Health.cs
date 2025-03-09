using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public UnityAction OnDie;

    public float GetRatio() => (float)currentHealth / MaxHealth;

    private bool isDead;
    private int currentHealth;

    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        HandleDeath();
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
    }

    private void HandleDeath()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            OnDie?.Invoke();
        }
    }
}