using UnityEngine;

public class ItStalker : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Animator animator;

    [Header("Flashlight")]
    public FlashlightToggle flashlightToggle;
    public float flashlightBlindAngle = 10f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float fleeSpeed = 6f;

    [Header("Attack")]
    public float attackDamage = 2.5f;
    public string attackTriggerName = "Attack";
    public float attackBeforeFleeDelay = 1.2f;

    [Header("Animation")]
    public string speedParameterName = "Speed";
    public float idleAnimSpeed = 0f;
    public float walkAnimSpeed = 1f;
    public float runAnimSpeed = 4f;
    public float animationSmoothTime = 0.2f;

    [Header("Vision Settings")]
    public float detectionDistance = 15f;

    [Header("Flee Behavior")]
    public float lingerMinTime = 2f;
    public float lingerMaxTime = 3f;
    public float fleeDistance = 20f;

    [Header("Respawn")]
    public float respawnDelay = 5f;
    public float respawnMinDistance = 25f;
    public float respawnMaxDistance = 45f;

    [Header("Proximity Audio")]
    public AudioClip helloClip;
    public AudioClip branchSnapClip;
    public float proximitySoundDistance = 12f;
    public float proximitySoundCooldown = 8f;
    public float soundVolume = 0.8f;

    private enum State
    {
        Stalking,
        Frozen,
        Attacking,
        Fleeing,
        WaitingToRespawn
    }

    private State currentState = State.Stalking;

    private float lingerTimer;
    private Vector3 fleeTarget;
    private float respawnTimer;
    private float proximitySoundTimer;

    private Renderer[] renderers;
    private Collider[] colliders;
    private AudioSource audioSource;
    private bool playHelloNext = true;
    private bool playerInSafeZone = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        proximitySoundTimer = Random.Range(2f, proximitySoundCooldown);
    }

    void Start()
    {
        if (flashlightToggle == null)
            flashlightToggle = FindObjectOfType<FlashlightToggle>();
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        HandleProximityAudio();

        switch (currentState)
        {
            case State.Stalking:
                SetAnimationSpeed(walkAnimSpeed);
                HandleStalking();
                break;

            case State.Frozen:
                SetAnimationSpeed(idleAnimSpeed);
                HandleFrozen();
                break;

            case State.Attacking:
                SetAnimationSpeed(idleAnimSpeed);
                FacePlayer();
                break;

            case State.Fleeing:
                SetAnimationSpeed(runAnimSpeed);
                HandleFleeing();
                break;

            case State.WaitingToRespawn:
                SetAnimationSpeed(idleAnimSpeed);
                HandleRespawnWaiting();
                break;
        }
    }

    void SetAnimationSpeed(float speed)
    {
        if (animator == null)
            return;

        animator.SetFloat(speedParameterName, speed, animationSmoothTime, Time.deltaTime);
    }

    void HandleStalking()
    {
        if (playerInSafeZone)
        {
            StartFleeing();
            return;
        }

        Vector3 directionToIt = (transform.position - playerCamera.position).normalized;

        float angle = Vector3.Angle(playerCamera.forward, directionToIt);
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        bool flashlightIsOn = flashlightToggle != null && flashlightToggle.IsActuallyOn();
        bool playerIsShiningLightAtIt = angle < flashlightBlindAngle;
        bool withinFlashlightRange = distance < detectionDistance;

        if (flashlightIsOn && playerIsShiningLightAtIt && withinFlashlightRange)
        {
            currentState = State.Frozen;
            lingerTimer = Random.Range(lingerMinTime, lingerMaxTime);
            return;
        }

        MoveTowardPlayer();
        FacePlayer();
    }

    void HandleFrozen()
    {
        if (playerInSafeZone)
        {
            StartFleeing();
            return;
        }

        lingerTimer -= Time.deltaTime;
        FacePlayer();

        if (lingerTimer <= 0f)
            StartFleeing();
    }

    void HandleFleeing()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            fleeTarget,
            fleeSpeed * Time.deltaTime
        );

        FaceAwayFromPlayer();

        if (Vector3.Distance(transform.position, fleeTarget) <= 0.5f)
        {
            HideIt();
            respawnTimer = respawnDelay;
            currentState = State.WaitingToRespawn;
        }
    }

    void HandleRespawnWaiting()
    {
        respawnTimer -= Time.deltaTime;

        if (respawnTimer <= 0f)
        {
            if (playerInSafeZone)
                return;

            RespawnNearPlayer();
            ShowIt();
            currentState = State.Stalking;
        }
    }

    void HandleProximityAudio()
    {
        if (currentState == State.WaitingToRespawn || playerInSafeZone)
            return;

        float distance = Vector3.Distance(playerCamera.position, transform.position);

        if (distance > proximitySoundDistance)
            return;

        proximitySoundTimer -= Time.deltaTime;

        if (proximitySoundTimer > 0f)
            return;

        if (playHelloNext && helloClip != null)
            audioSource.PlayOneShot(helloClip, soundVolume);
        else if (branchSnapClip != null)
            audioSource.PlayOneShot(branchSnapClip, soundVolume);

        playHelloNext = !playHelloNext;
        proximitySoundTimer = proximitySoundCooldown;
    }

    void MoveTowardPlayer()
    {
        Vector3 target = playerCamera.position;
        target.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    void AttackPlayer(PlayerHealth health)
    {
        if (playerInSafeZone)
            return;

        currentState = State.Attacking;

        if (health != null)
            health.TakeDamage(attackDamage);

        if (animator != null)
            animator.SetTrigger(attackTriggerName);

        CancelInvoke(nameof(StartFleeing));
        Invoke(nameof(StartFleeing), attackBeforeFleeDelay);
    }

    void StartFleeing()
    {
        if (playerCamera == null)
            return;

        currentState = State.Fleeing;
        SetAnimationSpeed(runAnimSpeed);

        Vector3 awayFromPlayer = transform.position - playerCamera.position;
        awayFromPlayer.y = 0f;

        if (awayFromPlayer.sqrMagnitude < 0.01f)
            awayFromPlayer = -transform.forward;

        awayFromPlayer.Normalize();

        fleeTarget = transform.position + awayFromPlayer * fleeDistance;
        fleeTarget.y = transform.position.y;

        FaceAwayFromPlayer();
    }

    void RespawnNearPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(respawnMinDistance, respawnMaxDistance);

        Vector3 spawnPosition = playerCamera.position + new Vector3(
            randomCircle.x * distance,
            0f,
            randomCircle.y * distance
        );

        spawnPosition.y = transform.position.y;
        transform.position = spawnPosition;

        proximitySoundTimer = Random.Range(1f, 3f);

        FacePlayer();
    }

    void HideIt()
    {
        SetAnimationSpeed(idleAnimSpeed);

        foreach (Renderer r in renderers)
            r.enabled = false;

        foreach (Collider c in colliders)
            c.enabled = false;
    }

    void ShowIt()
    {
        foreach (Renderer r in renderers)
            r.enabled = true;

        foreach (Collider c in colliders)
            c.enabled = true;
    }

    void FacePlayer()
    {
        Vector3 lookTarget = playerCamera.position;
        lookTarget.y = transform.position.y;

        transform.LookAt(lookTarget);
    }

    void FaceAwayFromPlayer()
    {
        Vector3 awayDirection = transform.position - playerCamera.position;
        awayDirection.y = 0f;

        if (awayDirection.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(awayDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentState == State.Fleeing ||
            currentState == State.WaitingToRespawn ||
            currentState == State.Attacking ||
            playerInSafeZone)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null)
        {
            Debug.LogWarning("ItStalker: PlayerHealth not found on player.", other);
            return;
        }

        Debug.Log("Player attacked!");
        AttackPlayer(health);
    }

    public void EnterSafeZone()
    {
        playerInSafeZone = true;

        CancelInvoke(nameof(StartFleeing));

        if (currentState != State.Fleeing &&
            currentState != State.WaitingToRespawn)
        {
            StartFleeing();
        }
    }

    public void ExitSafeZone()
    {
        playerInSafeZone = false;
    }
}