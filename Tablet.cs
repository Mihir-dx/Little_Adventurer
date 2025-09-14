using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoreTablet : MonoBehaviour
{
    [Header("Lore Content")]
    [TextArea(5, 10)]
    public string loreText;

    [Header("UI References")]
    public GameObject lorePanel;
    public TextMeshProUGUI loreTextDisplay;
    private bool prevCursorVisible;
    private CursorLockMode prevCursorLock;

    public void OnInteract()
    {
        if (lorePanel != null && loreTextDisplay != null)
        {
            loreTextDisplay.text = loreText;
            lorePanel.SetActive(true);
            Time.timeScale = 0f;

            // Save and enable cursor for UI interaction
            prevCursorVisible = Cursor.visible;
            prevCursorLock = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ClosePanel()
    {
        if (lorePanel != null && lorePanel.activeSelf)
        {
            lorePanel.SetActive(false);
            Time.timeScale = 1f;

            // Restore previous cursor state
            Cursor.visible = prevCursorVisible;
            Cursor.lockState = prevCursorLock;
        }
    }
}
