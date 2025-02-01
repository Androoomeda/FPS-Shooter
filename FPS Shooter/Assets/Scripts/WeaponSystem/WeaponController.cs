using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public int MaxAmmo = 8;

    [Tooltip("Задержка между выстрелами")]
    public float DelayBetweenShots = 0.2f;

    public float ReloadTime = 2f;

    public ProjectileBase ProjectilePrefab;

    [Tooltip("Дуло оружия")]
    public Transform WeaponMuzzle;

    [Tooltip("Уголо разброса")]
    public float BulletSpreadAngle = 0f;

    [Tooltip("Коэффицент во время прицеливания")] [Range(0f, 1f)]
    public float AimZoomRatio = 1f;

    public UnityAction OnShoot;

    public GameObject Owner { get; set; }

    public int GetCurrentAmmo() => Mathf.FloorToInt(currentAmmo);

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
            Debug.Log("Weapon is reload");
        }
    }

    private bool TryShoot()
    {
        if (currentAmmo > 0 && lastTimeShot + DelayBetweenShots < Time.time)
        {
            HandleShoot();
            currentAmmo -= 1;

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

        OnShoot?.Invoke();
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }
}