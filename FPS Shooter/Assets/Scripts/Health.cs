using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    public UnityAction OnDie;

    public float GetRatio() => (float)CurrentHealth / MaxHealth;

    private bool isDead;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        HandleDeath();
    }

    private void HandleDeath()
    {
        if (CurrentHealth <= 0)
        {
            isDead = true;
            OnDie?.Invoke();
        }
    }
}