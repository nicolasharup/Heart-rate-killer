using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float batteryAmount = 35f;

    [Header("Optional")]
    public AudioClip pickupSound;
    public float pickupVolume = 0.8f;

    private bool pickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        FlashlightToggle flashlight = FindObjectOfType<FlashlightToggle>();

        if (flashlight == null)
        {
            Debug.LogWarning("BatteryPickup: No FlashlightToggle found in scene.");
            return;
        }

        pickedUp = true;

        flashlight.AddBattery(batteryAmount);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        Destroy(gameObject);
    }
}