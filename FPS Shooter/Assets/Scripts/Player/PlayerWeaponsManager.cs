using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerWeaponsManager : MonoBehaviour
{
    public WeaponController weapon;

    [Tooltip("Speed at which the aiming animatoin is played")]
    public float AimingAnimationSpeed = 10f;
    [Tooltip("Position for weapons when active but not actively aiming")]
    public Transform WeaponParentSocket;
    [Tooltip("Position for weapons when active but not actively aiming")]
    public Transform DefaultWeaponPosition;
    [Tooltip("Position for weapons when aiming")]
    public Transform AimingWeaponPosition;

    public bool IsAiming { get; private set; }

    private PlayerInputHandler playerInput;
    private PlayerController playerLogic;
    private float DefaultFov = 60f;

    private void Start()
    {
        playerInput = GetComponent<PlayerInputHandler>();
        playerLogic = GetComponent<PlayerController>();

        weapon.Owner = gameObject;

        SetFov(DefaultFov);
    }

    private void Update()
    {
        weapon.HandleShootInput(playerInput.GetFireInputDown(), playerInput.GetFireInputHeld());
        UpdateWeaponAiming(playerInput.GetAimInputHeld());

        if (playerInput.GetReloadButtonDown())
            weapon.StartReload();
    }

    private void UpdateWeaponAiming(bool inputHeld)
    {
        Vector3 weaponPosition = WeaponParentSocket.localPosition;

        if (inputHeld)
        {
            weaponPosition = Vector3.Lerp(weaponPosition, AimingWeaponPosition.localPosition,
                AimingAnimationSpeed * Time.deltaTime);
            SetFov(Mathf.Lerp(playerLogic.PlayerCamera.fieldOfView, weapon.AimZoomRatio * DefaultFov,
                AimingAnimationSpeed * Time.deltaTime));
        }
        else
        {
            weaponPosition = Vector3.Lerp(weaponPosition,
                        DefaultWeaponPosition.localPosition, AimingAnimationSpeed * Time.deltaTime);
            SetFov(Mathf.Lerp(playerLogic.PlayerCamera.fieldOfView, DefaultFov, 
                AimingAnimationSpeed * Time.deltaTime));
        }

        WeaponParentSocket.localPosition = weaponPosition;
    }

    public void SetFov(float fov)
    {
        playerLogic.PlayerCamera.fieldOfView = fov;
    }

    public void SwitchWeapon(WeaponController weaponPrefab)
    {
        GameObject newWeapon = Instantiate(weaponPrefab.gameObject, 
            DefaultWeaponPosition.position, WeaponParentSocket.rotation, WeaponParentSocket);

        InventoryManager.Instance.AddItem(weapon.Item);
        Destroy(weapon.gameObject);

        weapon = newWeapon.GetComponent<WeaponController>();
        weapon.Owner = gameObject;
    }
}