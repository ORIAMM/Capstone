using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Combo
{
    int Combo_numb;
    public float AnimationFrames;
    public float FrameInWhichProjectileSpawn;
}
public class PlayerCombat : MonoBehaviour
{
    //public List<Combo> ProjectileTimeline = new();
    [HideInInspector] public Animator animator;
    [SerializeField] int TotalCombos = 0;
    public float ComboInterval = 0.3f;

    private Coroutine coroutine;
    float time = 0;
    int index = 0;

/*    public void Attack() => coroutine ??= StartCoroutine(Attacking());
    public IEnumerator Attacking()
    {
<<<<<<< Updated upstream
        *//*if (Time.time >= time || index + 1 >= combos.Count) index = 0;
=======
        if (Time.time >= time || index + 1 <= TotalCombos) index = 0;
>>>>>>> Stashed changes
        else index++;

        yield return null;
        //float ProjectileSpawnTime = ProjectileTimeline[index].type == Attack_Type.projectile ? ProjectileTimeline[index].FrameInWhichProjectileSpawn / ProjectileTimeline[index].AnimationFrames : 10;
        string animationName = "Attack" + (index + 1).ToString();

<<<<<<< Updated upstream
        LowerBody.Play(animationName);
        UpperBody.Play(animationName);
        yield return new WaitUntil(() => LowerBody.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        while (LowerBody.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            if (LowerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= ProjectileSpawnTime && !hasSpawned)
            {
                var obj = Instantiate(combos[index].Projectile, transform.position, transform.rotation);
                obj.GetComponent<Projectile>().StartBullet(transform.lossyScale.x);
                hasSpawned = true;
            }
            yield return null;
        }
        rb2d.mass -= MassIncreaseOnAttack;
        coroutine = null;
        time = Time.time + ComboInterval;*//*
    }*/
=======
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).IsName(animationName));
        //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).normalizedTime < ProjectileSpawnTime;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 0.9f);

        time = Time.time + ComboInterval;
        coroutine = null;
    }
>>>>>>> Stashed changes
}
