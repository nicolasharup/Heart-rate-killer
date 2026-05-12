using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 5f;
    public float currentHealth = 5f;

    [Header("Game Over")]
    public GameOverManager gameOverManager;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverManager == null)
            gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player health: " + currentHealth + " / " + maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player died!");

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
    }
}