using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    [SerializeField] private Image HealthFillImage;


    private Health playerHealth;

    void Start()
    {
        PlayerLogic player = FindFirstObjectByType<PlayerLogic>();
        playerHealth = player.GetComponent<Health>();
    }

    void Update()
    {
        HealthFillImage.fillAmount = (float)playerHealth.GetCurrentHealth() / playerHealth.MaxHealth;
    }
}
