using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    private Character cc;

    public float CurrentHealthPercentage
    {
        get{ 
        return (float)CurrentHealth / (float)MaxHealth;
        }
    }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        cc = GetComponent<Character>();
    }
    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        Debug.Log(gameObject.name + " Took Damage: " + damage);
        Debug.Log(gameObject.name + " Current Health: " + CurrentHealth);

        CheckHealth();
    }

    public void CheckHealth()
    {
        if(CurrentHealth <= 0)
        {
            cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int health)
    {
        CurrentHealth += health;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
}
