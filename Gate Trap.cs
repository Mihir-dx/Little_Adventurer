using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GateTrap : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Direction to move (normalized internally).")]
    public Vector3 moveDirection = Vector3.up;
    [Tooltip("Total travel distance from start to open position.")]
    public float moveDistance = 2f;
    [Tooltip("Movement speed (higher = faster ping-pong).")]
    public float moveSpeed = 6f;

    [Header("Kill settings")]
    [Tooltip("If true, kill instantly.")]
    public bool instantKill = true;

    private Vector3 _startPos;
    public AudioSource GTSound;
    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        var dir = moveDirection.sqrMagnitude > 0.0001f ? moveDirection.normalized : Vector3.up;
        float d = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = _startPos + dir * d;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<Health>();
        if (health != null)
        {
            if (instantKill)
                GTSound.Play();
                health.ApplyDamage(health.CurrentHealth);
        }
        else
        {
            // Fallback if no Health on player
            Destroy(other.gameObject);
        }
    }

    // Collide
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        var health = collision.gameObject.GetComponent<Health>();
        if (health != null)
            health.ApplyDamage(health.CurrentHealth);
        else
            Destroy(collision.gameObject);
    }
}
