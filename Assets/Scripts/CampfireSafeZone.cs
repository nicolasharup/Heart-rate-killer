using UnityEngine;

public class CampfireSafeZone : MonoBehaviour
{
    [Header("Healing")]
    public float healPerSecond = 1f;

    [Header("References")]
    public ItStalker clown;
    public RandomAmbiencePlayer ambiencePlayer;

    private bool playerInside = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        if (clown == null)
            clown = FindObjectOfType<ItStalker>();

        if (ambiencePlayer == null)
            ambiencePlayer = FindObjectOfType<RandomAmbiencePlayer>();
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (playerHealth != null)
            playerHealth.Heal(healPerSecond * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered safe zone.");

        playerInside = true;
        playerHealth = other.GetComponent<PlayerHealth>();

        if (clown != null)
            clown.EnterSafeZone();

        if (ambiencePlayer != null)
            ambiencePlayer.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player left safe zone.");

        playerInside = false;
        playerHealth = null;

        if (clown != null)
            clown.ExitSafeZone();

        if (ambiencePlayer != null)
            ambiencePlayer.enabled = true;
    }
}