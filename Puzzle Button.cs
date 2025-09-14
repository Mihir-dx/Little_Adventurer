using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public GateManager gateManager;
    public AudioSource pressSound;

    private bool isActivated = false;

    public void OnInteract()
    {
        if (!isActivated)
        {
            gateManager.ButtonPressed(this);
        }
    }

    public void Activate()
    {
        isActivated = true;
        if (pressSound != null)
        {
            pressSound.Play();
        }
    }

    public void ResetButton()
    {
        isActivated = false;
    }
}
