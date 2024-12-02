using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyI : MonoBehaviour, IEntity
{
    public float healthbar;
    [SerializeField] private PlayerCombat _PlayerCombat;
    public float def;

    public void ReceiveDamage(float value)
    {
        Debug.Log("JustinSayang");
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
