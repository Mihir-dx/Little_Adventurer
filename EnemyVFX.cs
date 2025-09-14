using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect FootStep;
    public VisualEffect Attack;
    public ParticleSystem BeingHit;      // scene particle on the enemy (optional)
    public VisualEffect BeingHitVFX;     // prefab for splash

    private VisualEffect _lastHitVfxInstance;

    public void PlayAttackVFX()
    {
        if (Attack != null) Attack.Play();
    }

    public void BurstFootStep()
    {
        if (FootStep != null) FootStep.Play();
    }

    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 dirFromAttacker = (transform.position - attackerPos);
        dirFromAttacker.y = 0f;
        if (dirFromAttacker.sqrMagnitude < 0.0001f) dirFromAttacker = transform.forward;
        dirFromAttacker.Normalize();

        Vector3 hitPos = transform.position;
        var col = GetComponent<Collider>();
        if (col != null)
            hitPos = col.ClosestPoint(attackerPos);

        hitPos.y += 0.5f;   // Slight vertical offset

        if (BeingHitVFX != null)
        {
            if (_lastHitVfxInstance != null)
                Destroy(_lastHitVfxInstance.gameObject);

            _lastHitVfxInstance = Instantiate(
                BeingHitVFX,
                hitPos,
                Quaternion.LookRotation(dirFromAttacker),
                transform
            );

            Destroy(_lastHitVfxInstance.gameObject, 2f);
        }
    }

    public void OnDeadCleanup()
    {
        if (_lastHitVfxInstance != null)
        {
            Destroy(_lastHitVfxInstance.gameObject);
            _lastHitVfxInstance = null;
        }

        if (BeingHit != null)
            BeingHit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
