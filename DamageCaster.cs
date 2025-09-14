using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;
    public int damage = 40;
    public string TargetTag;
    private List<Collider> _damageTargetList;

    private void Awake()
    {
        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        _damageTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag && !_damageTargetList.Contains(other))
        {
            Character targetcc = other.GetComponent<Character>();
            if (targetcc != null)
            {
                targetcc.ApplyDamage(damage, transform.parent.position);
                PlayerVFX playerVFX = transform.parent.GetComponent<PlayerVFX>();

                if(playerVFX != null)
                {
                    RaycastHit hit;

                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;

                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);

                    if(isHit)
                    {
                        playerVFX.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }
            _damageTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }

    public void DisableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }

}
