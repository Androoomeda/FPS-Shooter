using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        health.OnDie += OnDie;
    }

    void Update()
    {
    }

    private void OnDie()
    {
        Destroy(gameObject);
    }
}
