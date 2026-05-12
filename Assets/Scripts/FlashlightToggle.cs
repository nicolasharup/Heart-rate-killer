using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightToggle : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform clown;
    public GameObject physicalFlashlight;

    private Light flashlight;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float batteryDrainPerSecond = 2f;

    [Header("Flicker Distance")]
    public float flickerStartDistance = 25f;
    public float heavyFlickerDistance = 6f;

    [Header("Flicker Timing")]
    public float farFlickerInterval = 0.6f;
    public float closeFlickerInterval = 0.08f;

    [Header("Off Blink Duration")]
    public float farOffTime = 0.04f;
    public float closeOffTime = 0.18f;

    private bool playerWantsFlashlightOn = true;
    private bool flickerOff = false;

    private float flickerTimer;
    private float offTimer;

    void Awake()
    {
        flashlight = GetComponent<Light>();

        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        if (flashlight != null)
            playerWantsFlashlightOn = flashlight.enabled;

        ApplyPhysicalFlashlightState();
    }

    void Update()
    {
        HandleToggle();
        HandleBatteryDrain();
        HandleFlicker();
        ApplyPhysicalFlashlightState();
    }

    void HandleToggle()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (currentBattery <= 0f)
            {
                playerWantsFlashlightOn = false;
                return;
            }

            playerWantsFlashlightOn = !playerWantsFlashlightOn;
        }
    }

    void HandleBatteryDrain()
    {
        if (!playerWantsFlashlightOn)
            return;

        if (currentBattery <= 0f)
        {
            currentBattery = 0f;
            playerWantsFlashlightOn = false;
            return;
        }

        currentBattery -= batteryDrainPerSecond * Time.deltaTime;
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        if (currentBattery <= 0f)
        {
            playerWantsFlashlightOn = false;
            flickerOff = false;

            if (flashlight != null)
                flashlight.enabled = false;
        }
    }

    void HandleFlicker()
    {
        if (flashlight == null)
            return;

        if (!playerWantsFlashlightOn || currentBattery <= 0f)
        {
            flashlight.enabled = false;
            return;
        }

        if (player == null || clown == null)
        {
            flashlight.enabled = true;
            return;
        }

        float distance = Vector3.Distance(player.position, clown.position);

        if (distance > flickerStartDistance)
        {
            flickerOff = false;
            flashlight.enabled = true;
            return;
        }

        float closeness = Mathf.InverseLerp(
            flickerStartDistance,
            heavyFlickerDistance,
            distance
        );

        if (flickerOff)
        {
            offTimer -= Time.deltaTime;

            if (offTimer <= 0f)
            {
                flickerOff = false;
                flashlight.enabled = true;
            }

            return;
        }

        flickerTimer -= Time.deltaTime;

        if (flickerTimer <= 0f)
        {
            flickerOff = true;
            flashlight.enabled = false;

            offTimer = Mathf.Lerp(
                farOffTime,
                closeOffTime,
                closeness
            );

            flickerTimer = Mathf.Lerp(
                farFlickerInterval,
                closeFlickerInterval,
                closeness
            );
        }
    }

    void ApplyPhysicalFlashlightState()
    {
        if (physicalFlashlight != null)
            physicalFlashlight.SetActive(playerWantsFlashlightOn && currentBattery > 0f);
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        Debug.Log("Battery added. Current battery: " + currentBattery);
    }

    public bool IsActuallyOn()
    {
        return flashlight != null &&
               flashlight.enabled &&
               playerWantsFlashlightOn &&
               currentBattery > 0f;
    }

    public float GetBatteryPercent()
    {
        if (maxBattery <= 0f)
            return 0f;

        return currentBattery / maxBattery;
    }
}