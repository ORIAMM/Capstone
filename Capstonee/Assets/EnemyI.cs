using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyI : MonoBehaviour, IEntity
{
    public float healthbar;

    public float def;

    public void ReceiveDamage(float value)
    {
        healthbar -= (value - def);
    }
    public void OnDeath()
    {
        if (healthbar <= 0)
        {
            Debug.Log("Ambatublow");
        }
    }
}
