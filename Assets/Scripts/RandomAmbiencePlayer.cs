using UnityEngine;

public class RandomAmbiencePlayer : MonoBehaviour
{
    [Header("Bass Clips")]
    public AudioClip[] creepyClips;

    [Header("References")]
    public Transform player;
    public Transform clown;

    [Header("Audio")]
    public float volume = 0.7f;

    [Header("Distance Settings")]
    public float maxDistance = 40f;

    [Header("Pulse Timing")]
    public float farInterval = 4f;
    public float closeInterval = 0.25f;

    [Header("Pitch Variation")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private AudioSource audioSource;
    private float timer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (player == null || clown == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            clown.position
        );

        // Too far away = silence
        if (distance > maxDistance)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayPulse(distance);
        }
    }

    void PlayPulse(float distance)
    {
        if (creepyClips == null || creepyClips.Length == 0)
            return;

        AudioClip clip = creepyClips[
            Random.Range(0, creepyClips.Length)
        ];

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(clip, volume);

        // Closer clown = faster pulses
        float t = distance / maxDistance;

        timer = Mathf.Lerp(
            closeInterval,
            farInterval,
            t
        );
    }
}