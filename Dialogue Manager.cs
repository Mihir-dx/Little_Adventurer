using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The parent dialogue box panel.")]
    public GameObject dialogueBox;
    [Tooltip("The TextMeshPro object for displaying the dialogue lines.")]
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Content")]
    [Tooltip("Write the dialogue lines here. Each element is a new line.")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    [Header("Player Reference")]
    [Tooltip("Drag the Player GameObject here to disable their movement.")]
    public Character playerCharacter;

    private int currentLine = 0;
    private bool isDialogueActive = false;

    void Start()
    {
        StartDialogue();
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        isDialogueActive = true;

        if (playerCharacter != null)
        {
            playerCharacter.enabled = false;
        }

        dialogueBox.SetActive(true);
        currentLine = 0;
        StartCoroutine(TypeLine(dialogueLines[currentLine]));
    }

    private void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        dialogueBox.SetActive(false);

        if (playerCharacter != null)
        {
            playerCharacter.enabled = true;
        }
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f); // Adjust typing speed here
        }
    }
}
