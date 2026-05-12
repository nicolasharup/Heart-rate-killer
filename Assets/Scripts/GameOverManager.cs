using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;

    void Start()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        LockCursor();
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        LockCursor();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}