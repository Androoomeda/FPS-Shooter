using System;
using UnityEngine;

public class WeaponController : MonoBehaviour 
{
    public bool IsReloading;

    public bool AutomaticReload;
    internal float CurrentAmmoRatio;
    internal object SourcePrefab;

    public Vector3 RecoilForce { get; internal set; }
    public bool IsCharging { get; internal set; }
    public Vector3 AimOffset { get; internal set; }
    public float AimZoomRatio { get; internal set; }
    public GameObject Owner { get; internal set; }

    internal bool HandleShootInputs(bool v1, bool v2, bool v3)
    {
        throw new NotImplementedException();
    }

    internal void ShowWeapon(bool v)
    {
        throw new NotImplementedException();
    }

    internal void StartReloadAnimation()
    {
        throw new NotImplementedException();
    }
}