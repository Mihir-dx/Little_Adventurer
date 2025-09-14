using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    [Tooltip("How far the gate should move up when opened.")]
    public float openHeight = 5f;

    [Tooltip("How fast the gate opens.")]
    public float openSpeed = 2f;

    [Tooltip("Sound to play when the gate opens.")]
    public AudioSource openingSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;

    void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
    }

    public void OpenGate()
    {
        if (!isOpening)
        {
            if (openingSound != null)
            {
                openingSound.Play();
            }
            StartCoroutine(AnimateGate());
        }
    }

    private IEnumerator AnimateGate()
    {
        isOpening = true;
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = openPosition;
    }
}

