using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Required for using Lists

public class GateManager : MonoBehaviour
{
    [Header("Puzzle Components")]
    [Tooltip("All the gates that this manager will open.")]
    public List<GateController> gatesToOpen;
    [Tooltip("The first interaction zone for this puzzle.")]
    public PuzzleButton button1;
    [Tooltip("The second interaction zone for this puzzle.")]
    public PuzzleButton button2;

    [Header("Timer Settings")]
    [Tooltip("How many seconds the player has to activate the second button.")]
    public float timeLimit = 5f;
    [Tooltip("Optional: A UI Slider to show the remaining time.")]
    public Slider timerSlider;

    [Header("Audio Feedback")]
    [Tooltip("Optional: Sound to play on success.")]
    public AudioSource puzzleSuccessSound;
    [Tooltip("Optional: Sound to play on failure.")]
    public AudioSource puzzleFailSound;

    private PuzzleButton firstButtonPressed = null;
    private float countdownTimer;
    private bool isTimerRunning = false;
    private bool isPuzzleSolved = false;

    void Start()
    {
        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            countdownTimer -= Time.deltaTime;
            if (timerSlider != null)
            {
                timerSlider.value = countdownTimer / timeLimit;
            }

            if (countdownTimer <= 0)
            {
                ResetPuzzle();
            }
        }
    }

    public void ButtonPressed(PuzzleButton pressedButton)
    {
        if (isPuzzleSolved) return;

        if (firstButtonPressed == null)
        {
            firstButtonPressed = pressedButton;
            pressedButton.Activate();
            isTimerRunning = true;
            countdownTimer = timeLimit;
            if (timerSlider != null)
            {
                timerSlider.gameObject.SetActive(true);
            }
        }
        else if (pressedButton != firstButtonPressed)
        {
            pressedButton.Activate();
            isPuzzleSolved = true;
            isTimerRunning = false;
            if (timerSlider != null)
            {
                timerSlider.gameObject.SetActive(false);
            }

            // Loop through every gate in the list and open it.
            foreach (GateController gate in gatesToOpen)
            {
                gate.OpenGate();
            }

            if (puzzleSuccessSound != null)
            {
                puzzleSuccessSound.Play();
            }
        }
    }

    private void ResetPuzzle()
    {
        isTimerRunning = false;
        firstButtonPressed = null;
        button1.ResetButton();
        button2.ResetButton();

        if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(false);
        }

        if (puzzleFailSound != null)
        {
            puzzleFailSound.Play();
        }
    }
}
