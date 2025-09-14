using UnityEngine;

public class PickUp : MonoBehaviour
{
    public PickUpType Type;

    [Header("Per-type values")]
    [SerializeField] private int healValue = 20;
    [SerializeField] private int coinValue = 5;
    [HideInInspector] public int Value;

    public ParticleSystem CoinCollectedVFX;
    public AudioSource HealSFX;

    public enum PickUpType
    {
        Heal,
        Coin
    }

    private void Awake()
    {
        SyncValue();
    }

    private void OnValidate()
    {
        SyncValue();
    }

    private void SyncValue()
    {
        Value = (Type == PickUpType.Heal) ? healValue : coinValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var character = other.GetComponent<Character>();
        if (character != null)
            character.PickUpItem(this);

        if (Type == PickUpType.Heal && HealSFX != null && HealSFX.clip != null)
        {
            AudioSource.PlayClipAtPoint(HealSFX.clip, transform.position, HealSFX.volume);
        }

        if (CoinCollectedVFX != null)
        {
            Instantiate(CoinCollectedVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
