using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The UI Text object that says 'Press E to Interact'.")]
    public TextMeshProUGUI interactPrompt;

    // References to the objects we can currently interact with
    private PuzzleButton currentButton = null;
    private LoreTablet currentTablet = null;

    void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentButton != null)
            {
                currentButton.OnInteract();
            }
            else if (currentTablet != null)
            {
                currentTablet.OnInteract();
                HideInteractPrompt();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuzzleButton"))
        {
            currentButton = other.GetComponent<PuzzleButton>();
            if (interactPrompt != null) { interactPrompt.gameObject.SetActive(true); }
        }
        else if (other.CompareTag("LoreTablet"))
        {
            currentTablet = other.GetComponent<LoreTablet>();
            if (interactPrompt != null) { interactPrompt.gameObject.SetActive(true); }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PuzzleButton"))
        {
            currentButton = null;
            if (interactPrompt != null) { interactPrompt.gameObject.SetActive(false); }
        }
        else if (other.CompareTag("LoreTablet"))
        {
            currentTablet = null;
            if (interactPrompt != null) { interactPrompt.gameObject.SetActive(false); }
        }
    }

    public void HideInteractPrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.gameObject.SetActive(false);
        }

        currentButton = null;
        currentTablet = null;
    }
}
