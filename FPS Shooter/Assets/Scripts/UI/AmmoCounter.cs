using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CountText;
    [SerializeField] private Image ReloadFillImage;

    private PlayerWeaponsManager playerWeaponsManager;
    private WeaponController weapon;

    void Start()
    {
        playerWeaponsManager = FindFirstObjectByType<PlayerWeaponsManager>();
        weapon = playerWeaponsManager.weapon;
        ReloadFillImage.fillAmount = 0f;
    }

    void Update()
    {
        weapon = playerWeaponsManager.weapon;
        CountText.text = $"{weapon.GetCurrentAmmo()}/{InventoryManager.Instance.AmmoAmount}";

        UpdateReloadingBar();
    }

    private void UpdateReloadingBar()
    {
        if(weapon.IsReloading)
            ReloadFillImage.fillAmount = weapon.GetRealoadingProgress();
        else
            ReloadFillImage.fillAmount = 0f;
    }
}
