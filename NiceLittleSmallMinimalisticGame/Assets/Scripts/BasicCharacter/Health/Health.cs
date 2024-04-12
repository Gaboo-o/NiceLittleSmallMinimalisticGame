using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //Components
    [SerializeField]
    private HealthBar hpBar;

    //Vars
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    public int curHealth;
    public bool isInvencible = false;

    //Functions
    public void TakeDamage(int dmg)
    {
        if (!isInvencible)
        {
            curHealth -= dmg;

            hpBar.SetHealth(curHealth);
        }
        Dying();
    }
    public void Healing(int heal)
    {
        curHealth += heal;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }

        hpBar.SetHealth(curHealth);
    }
    public void Dying()
    {
        if (curHealth <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    //Functions (Trigger & Input)
    public void OnTakeDamage(int dmg)
    {
        TakeDamage(dmg);
    }
    public void OnHealing(int heal)
    {
        Healing(heal);
    }

    //InGame
    private void Start()
    {
        curHealth = maxHealth;
        hpBar.SetMaxHealth(maxHealth);
    }
}