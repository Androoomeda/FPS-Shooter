using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image AmmoFillImage;

    private PlayerWeaponsManager playerWeaponsManager;
    private WeaponController weapon;

    void Start()
    {
        playerWeaponsManager = FindFirstObjectByType<PlayerWeaponsManager>();
        weapon = playerWeaponsManager.weapon;
    }

    void Update()
    {
        weapon = playerWeaponsManager.weapon;

        AmmoFillImage.fillAmount = (float)weapon.GetCurrentAmmo() / weapon.MaxAmmo;

        if(weapon.GetCurrentAmmo() <= 0)
            AmmoFillImage.fillAmount = weapon.GetRealoadingProgress();
    }
}
