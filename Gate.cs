using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject GateVisual;
    private Collider GateCollider;
    public float OpenDuration = 2f;
    public float OpenTargetY = -1.5f;

    private void Awake()
    {
        GateCollider = GetComponent<Collider>();
    }

    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration = 0;
        Vector3 startPosition = GateVisual.transform.position;
        Vector3 targetPos = startPosition + Vector3.up * OpenTargetY;

        while (currentOpenDuration < OpenDuration)
        {
            currentOpenDuration += Time.deltaTime;
            GateVisual.transform.position = Vector3.Lerp(startPosition, targetPos, currentOpenDuration / OpenDuration);
            yield return null;
        }

        GateCollider.enabled = false;
    }

    public void Open()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
