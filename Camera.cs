using UnityEngine;

public class Camere : MonoBehaviour
{
    public GameObject Target;

    [Header("Camera Shake")]
    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultStrength = 0.2f;

    private Vector3 offset;
    private Vector3 shakeOffset;

    private Coroutine shakeRoutine;
    private bool isContinuous;
    private float continuousStrength;

    void Start()
    {
        if (Target != null)
            offset = transform.position - Target.transform.position;
    }

    void Update()
    {
        if (Target == null) return;

        // follow + shake
        transform.position = Target.transform.position + offset + shakeOffset;
    }

    // One-shot shake
    public void Shake(float duration, float strength)
    {
        if (isContinuous) return; // ignore if continuous shake is active
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeCo(duration, strength));
    }

    // Overload using defaults
    public void Shake()
    {
        Shake(defaultDuration, defaultStrength);
    }

    // Start continuous shake (until StopShake is called)
    public void StartContinuousShake()
    {
        StartContinuousShake(defaultStrength);
    }

    public void StartContinuousShake(float strength)
    {
        continuousStrength = strength;
        if (isContinuous) return;
        isContinuous = true;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        shakeRoutine = StartCoroutine(ContinuousShakeCo());
    }

    // Stop any shake (continuous or one-shot)
    public void StopShake()
    {
        isContinuous = false;
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }
        shakeOffset = Vector3.zero;
    }

    private System.Collections.IEnumerator ShakeCo(float duration, float strength)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float damper = 1f - (t / duration); // ease out
            Vector3 rnd = Random.insideUnitSphere * strength * damper;
            rnd.z = 0f;
            shakeOffset = rnd;
            yield return null;
        }
        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }

    private System.Collections.IEnumerator ContinuousShakeCo()
    {
        while (isContinuous)
        {
            Vector3 rnd = Random.insideUnitSphere * continuousStrength;
            rnd.z = 0f;
            shakeOffset = rnd;
            yield return null; // update each frame
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}
