using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float Speed = 2f;
    public int Damage = 10;
    public ParticleSystem HitVFX;
    private Rigidbody rb;
    private Collider myCollider;
    private Collider[] _ignored;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Init(GameObject owner)
    {
        if (owner == null || myCollider == null) return;

        _ignored = owner.GetComponentsInChildren<Collider>();
        for (int i = 0; i < _ignored.Length; i++)
        {
            if (_ignored[i] != null)
                Physics.IgnoreCollision(myCollider, _ignored[i], true);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_ignored != null)
        {
            for (int i = 0; i < _ignored.Length; i++)
                if (other == _ignored[i]) return;
        }

        Character CC = other.GetComponent<Character>();
        if (CC != null && CC.isPlayer)
        {
            CC.ApplyDamage(Damage, transform.position);
        }

        if (HitVFX != null)
            Instantiate(HitVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
