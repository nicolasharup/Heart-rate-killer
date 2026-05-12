using UnityEngine;
using UnityEngine.UI;

public class FlashlightPowerUI : MonoBehaviour
{
    public FlashlightToggle flashlight;
    public Slider powerSlider;

    void Start()
    {
        if (flashlight == null)
            flashlight = FindObjectOfType<FlashlightToggle>();

        if (powerSlider == null)
            powerSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (flashlight == null || powerSlider == null)
            return;

        powerSlider.value = flashlight.GetBatteryPercent();
    }
}