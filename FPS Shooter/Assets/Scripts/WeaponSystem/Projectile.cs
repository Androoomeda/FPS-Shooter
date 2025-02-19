using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : ProjectileBase
{
    [SerializeField] private float MaxLifeTime = 2f;
    [SerializeField] private float Radius = 0.01f;
    [SerializeField] private float Speed = 20f;
    [SerializeField] private int Damage = 40;
    [SerializeField] private float TrajectoryCorrectionDistance = -1;
    [SerializeField] private LayerMask HittableLayers = -1;

    Vector3 lastPosition;
    bool hasTrajectoryOverride;
    Vector3 trajectoryCorrectionVector;
    Vector3 consumedTrajectoryCorrectionVector;
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
        lastPosition = transform.position;
        ignoredColliders = new List<Collider>();

        Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
        ignoredColliders.AddRange(ownerColliders);

        hasTrajectoryOverride = true;
        Vector3 cameraToMuzzle = InitialPosition -
                                          Camera.main.transform.position;
        trajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                    Camera.main.transform.forward);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        if(hasTrajectoryOverride && consumedTrajectoryCorrectionVector.sqrMagnitude <
        trajectoryCorrectionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = trajectoryCorrectionVector - consumedTrajectoryCorrectionVector;
            float distanceThisFrame = (transform.position - lastPosition).magnitude;
            Vector3 correctionThisFrame = distanceThisFrame / TrajectoryCorrectionDistance * trajectoryCorrectionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);

            consumedTrajectoryCorrectionVector += correctionThisFrame;

            if (consumedTrajectoryCorrectionVector.sqrMagnitude == trajectoryCorrectionVector.sqrMagnitude)
                    hasTrajectoryOverride = false;

                transform.position += correctionThisFrame;
        }

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

        lastPosition = transform.position;
    }

    private bool IsHitValid(Collider hit)
    {
        if (hit.isTrigger && hit.GetComponent<Health>() == null)
            return false;

        if (ignoredColliders != null && ignoredColliders.Contains(hit))
            return false;

        return true;
    }

    private void OnHit(Collider collider)
    {
        Health health = collider.GetComponent<Health>();
        
        if(health)
            health.TakeDamage(Damage);

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
