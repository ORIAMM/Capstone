using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyI : MonoBehaviour, IEntity
{
    public BoxCollider Collider;

    public void OnCollisionEnter(Collision collision)
    {
        

        
    }

    public void ReceiveDamage(float value)
    {

    }
    public void OnDeath()
    {

    }
}
