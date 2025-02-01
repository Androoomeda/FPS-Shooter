using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : ProjectileBase
{
    [SerializeField] private float MaxLifeTime = 2f;
    [SerializeField] private float Radius = 0.01f;
    [SerializeField] private float Speed = 20f;
    [SerializeField] private float Damage = 40f;
    [SerializeField] private LayerMask HittableLayers = -1;

    private Vector3 velocity;
    private ProjectileBase projectileBase;
    private List<Collider> ignoredColliders;

    private void OnEnable()
    {
        projectileBase = GetComponent<ProjectileBase>();

        projectileBase.OnShoot += OnShoot;

        Destroy(gameObject, MaxLifeTime);
    }

    new void OnShoot()
    {
        velocity = transform.forward * Speed;
        ignoredColliders = new List<Collider>();

        Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
        ignoredColliders.AddRange(ownerColliders);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        Collider closestHit = new Collider();
        bool foundHit = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, Radius,
            HittableLayers, QueryTriggerInteraction.Collide);
        foreach (var hit in hits)
        {
            if (IsHitValid(hit))
            {
                foundHit = true;
                closestHit = hit;
            }
        }

        if (foundHit)
        {
            OnHit(closestHit);
        }
    }

    private bool IsHitValid(Collider hit)
    {
        if (hit.isTrigger && hit.GetComponent<Damageable>() == null)
            return false;

        if (ignoredColliders != null && ignoredColliders.Contains(hit))
            return false;  

        return true;
    }

    private void OnHit(Collider collider)
    {
        //TODO: сделать рег урона
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, Radius);
        }
    }
}
