using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite halfHeart;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (playerHealth == null)
            return;

        float health = playerHealth.currentHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            float heartValue = health - i;

            if (heartValue >= 1f)
            {
                hearts[i].enabled = true;
                hearts[i].sprite = fullHeart;
            }
            else if (heartValue >= 0.5f)
            {
                hearts[i].enabled = true;
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}