using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyI : TimedObject, IEntity
{
    public float healthbar;

    public float def;

    public new Coroutine TimeStopped;

    public Animator animator;

    public TimeManager timeManager;




    public override void OnContinue()
    {

        if (animator != null)
        {
            animator.speed = 1; // Resume the animation
        }
    }
    private void Update()
    {
        // if (TimeManager.instance.isStopped || TimeStopped != null) TimeStopped ??= StartCoroutine(OnStop());
    }

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

    public override void OnStop()
    {
        throw new System.NotImplementedException();
    }
}
