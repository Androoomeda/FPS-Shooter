using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    [SerializeField] private Image HealthFillImage;
    [SerializeField] private Image HealthTransitionFillImage;
    [SerializeField] private float fillSpeed = 0.2f;

    private Health playerHealth;

    void Start()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        playerHealth = player.GetComponent<Health>();
    }

    void Update()
    {
        HealthFillImage.fillAmount = playerHealth.GetRatio();

        if(playerHealth.GetRatio() > HealthTransitionFillImage.fillAmount)
            HealthTransitionFillImage.fillAmount = playerHealth.GetRatio();
        else 
            HealthTransitionFillImage.fillAmount -= fillSpeed * Time.deltaTime;
    }
}
