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
    public Collider Weapon;

    public bool isAttacking {  
        get ; 
        private set; 
    }
    public bool isBlocking
    {
        get;
        private set;
    }
    public Coroutine coroutine { get; private set; }
    float time = 0;
    int index = 0;

    public void Attack() => coroutine ??= StartCoroutine(Attacking());

    public void Block() => coroutine ??= StartCoroutine(Blocking());
    public IEnumerator Attacking()
    {
        Debug.Log("philipsGants");
        if (Time.time >= time || index + 1 <= TotalCombos)
        {
            index = 0;
            /*animator.SetBool("combatBreak", true);*/
        }
        else 
        {
            index++;
            /*animator.SetBool("combatBreak", false);*/
        }
        
        isAttacking = true;
        Debug.Log(index);

        yield return null;
        //float ProjectileSpawnTime = ProjectileTimeline[index].type == Attack_Type.projectile ? ProjectileTimeline[index].FrameInWhichProjectileSpawn / ProjectileTimeline[index].AnimationFrames : 10;
        string animationName = "Attack" + (index + 1).ToString();
        animator.Play(animationName);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).IsName(animationName));
        //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).normalizedTime < ProjectileSpawnTime;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f);

        isAttacking = false;
        time = Time.time + ComboInterval;
        coroutine = null;
    }

    public IEnumerator Blocking()
    {
        Debug.Log("Bloc");
        if (isBlocking == false)
        {
            isBlocking = true;
            animator.SetBool("isBlock", isBlocking);
            yield return new WaitForSeconds(2f);

            isBlocking = false;
            animator.SetBool("isBlock", isBlocking);
        }
        coroutine = null;


    }

    public void Impact()
    {
        StopCoroutine(Blocking());
        animator.SetTrigger("Impact");
        isBlocking = false;
        coroutine = null;
    }


}
