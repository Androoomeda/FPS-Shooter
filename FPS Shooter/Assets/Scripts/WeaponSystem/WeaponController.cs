using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public Item Item;
    public int MaxAmmo = 8;

    [Tooltip("Задержка между выстрелами")]
    public float DelayBetweenShots = 0.2f;
    public float ReloadTime = 2f;
    public ProjectileBase ProjectilePrefab;
    public Transform WeaponMuzzle;
    public float BulletSpreadAngle = 0f;

    [Tooltip("Коэффицент зума во время прицеливания")] [Range(0f, 1f)]
    public float AimZoomRatio = 1f;
    public GameObject Owner { get; set; }
    public int GetCurrentAmmo() => currentAmmo;
    public float GetRealoadingProgress() => (Time.time - lastTimeShot) / ReloadTime;

    private int currentAmmo;
    private bool isReloading;
    private float lastTimeShot;

    void Awake()
    {
        currentAmmo = MaxAmmo;
    }

    void Update()
    {
        if(currentAmmo == 0)
            isReloading = true;

        Reload();
    }

    public void HandleShootInput(bool inputDown, bool inputHeld)
    {
        if (inputDown || inputHeld)
            TryShoot();
    }

    public void StartReload()
    {
        if (currentAmmo < MaxAmmo)
        {
            isReloading = true;
        }
    }

    private void Reload()
    {
        if (isReloading && lastTimeShot + ReloadTime < Time.time)
        {
            currentAmmo = MaxAmmo;
            isReloading = false;
        }
    }

    private bool TryShoot()
    {
        if (currentAmmo > 0 && lastTimeShot + DelayBetweenShots < Time.time)
        {
            HandleShoot();
            currentAmmo--;

            return true;
        }

        return false;
    }

    private void HandleShoot()
    {
        Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
        ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position,
            Quaternion.LookRotation(shotDirection));
        newProjectile.Shoot(this);

        lastTimeShot = Time.time;
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }
}