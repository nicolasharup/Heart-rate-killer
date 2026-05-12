using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class StaminaUI : MonoBehaviour
{
    public FirstPersonController playerController;
    public Slider staminaSlider;

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<FirstPersonController>();

        if (staminaSlider == null)
            staminaSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerController == null || staminaSlider == null)
            return;

        staminaSlider.value = playerController.GetStaminaPercent();
    }
}