using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject tutorialScreen;
    public GameObject winScreen;

    [Header("Player")]
    public GameObject playerCapsule;
    public GameObject playerFollowCamera;

    [Header("Notes")]
    public TextMeshProUGUI noteCounterText;
    public int totalNotes = 7;

    private int collectedNotes = 0;
    private bool hasWon = false;

    void Start()
    {
        ShowCursor();
        Time.timeScale = 0f;

        if (startScreen != null)
            startScreen.SetActive(true);

        if (tutorialScreen != null)
            tutorialScreen.SetActive(false);

        if (winScreen != null)
            winScreen.SetActive(false);

        if (playerCapsule != null)
            playerCapsule.SetActive(false);

        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(false);

        if (noteCounterText != null)
        {
            noteCounterText.gameObject.SetActive(false);
            UpdateNoteCounter();
        }
    }

    void Update()
    {
        if ((startScreen != null && startScreen.activeSelf) ||
            (tutorialScreen != null && tutorialScreen.activeSelf) ||
            (winScreen != null && winScreen.activeSelf))
        {
            ShowCursor();
        }
    }

    public void ShowTutorial()
    {
        ShowCursor();

        if (startScreen != null)
            startScreen.SetActive(false);

        if (tutorialScreen != null)
            tutorialScreen.SetActive(true);
    }

    public void StartGame()
    {
        if (tutorialScreen != null)
            tutorialScreen.SetActive(false);

        if (playerCapsule != null)
            playerCapsule.SetActive(true);

        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(true);

        if (noteCounterText != null)
            noteCounterText.gameObject.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CollectNote()
    {
        if (hasWon)
            return;

        collectedNotes++;
        collectedNotes = Mathf.Clamp(collectedNotes, 0, totalNotes);

        UpdateNoteCounter();

        if (collectedNotes >= totalNotes)
            WinGame();
    }

    void WinGame()
    {
        hasWon = true;

        if (winScreen != null)
            winScreen.SetActive(true);

        if (noteCounterText != null)
            noteCounterText.gameObject.SetActive(false);

        Time.timeScale = 0f;
        ShowCursor();
    }

    void UpdateNoteCounter()
    {
        if (noteCounterText != null)
            noteCounterText.text = "Notes: " + collectedNotes + "/" + totalNotes;
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}