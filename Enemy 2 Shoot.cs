using UnityEngine;

public class Enemy2Shoot : MonoBehaviour
{
    public Transform ShootingPoint;
    public GameObject DamageOrb;
    public float SpawnForwardOffset = 0.3f;
    private Character cc;

    private void Awake()
    {
        cc = GetComponent<Character>();
    }

    public void ShootTheDamageOrb()
    {
        if (ShootingPoint == null || DamageOrb == null) return;

        Vector3 spawnPos = ShootingPoint.position + ShootingPoint.forward * SpawnForwardOffset;
        Quaternion rot = ShootingPoint.rotation;

        GameObject go = Instantiate(DamageOrb, spawnPos, rot);

        var orb = go.GetComponent<DamageOrb>();
        if (orb != null)
            orb.Init(gameObject);
    }

    private void Update()
    {
        cc.RotateToTarget();
    }
}
