using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NoteInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject noteUI;
    public GameObject interactText;
    public Image noteImage;

    [Header("This Note")]
    public Sprite noteToShow;

    private bool playerInRange = false;
    private bool noteOpen = false;
    private MainMenuManager menuManager;

    void Start()
    {
        menuManager = FindObjectOfType<MainMenuManager>();

        if (noteUI != null)
            noteUI.SetActive(false);

        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (playerInRange && !noteOpen)
                OpenNote();
        }

        if (noteOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseNoteAndCollect();
    }

    void OpenNote()
    {
        if (noteImage != null && noteToShow != null)
            noteImage.sprite = noteToShow;

        if (noteUI != null)
            noteUI.SetActive(true);

        if (interactText != null)
            interactText.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        noteOpen = true;
    }

    void CloseNoteAndCollect()
    {
        if (noteUI != null)
            noteUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        noteOpen = false;

        if (menuManager != null)
            menuManager.CollectNote();

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactText != null)
            interactText.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactText != null)
            interactText.SetActive(false);
    }
}