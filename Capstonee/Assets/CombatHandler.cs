using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct BasicAttackInfo
{
    [Header("Hit Frame Settings")]
    public float min_frameTime;
    public float max_frameTime;
    [Space(3)]
    public float MaxFrames;
}
public class CombatHandler : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] List<BasicAttackInfo> attackInfos;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] int TotalCombos = 0;

    [Header("Other Settings")]
    [SerializeField] private float DodgeCooltime;
    public float ComboInterval = 0.3f;

    [HideInInspector] public Animator animator;

    //Reference
    private Player2 _player;
    private ImprovisedPlayerMovement2 IPmovement;
    private HashSet<IEntity> hitEnemies = new HashSet<IEntity>();

    [HideInInspector] public Transform player;

    public bool isAttacking { get; private set; }
    public bool isBlocking { get; private set; }
    public bool isDodging { get; private set; }
    public bool isFall { get; private set; }
    public Coroutine coroutine { get; private set; }
    float time = 0;
    int index = 0;
    float dodgeTime = 0;
    bool dodgeCooldown => Time.time >= dodgeTime;

    float min_frameTime, max_frameTime;
    string animationName = "a";

    private void Start()
    {
        IPmovement = GetComponent<ImprovisedPlayerMovement2>();
        _player = GetComponent<Player2>();
    }
    public void Update()
    {
        Debug.Log(animationName + $"{max_frameTime - animator.GetCurrentAnimatorStateInfo(0).normalizedTime} <= {max_frameTime - min_frameTime}");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            if (max_frameTime - animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= max_frameTime - min_frameTime)
            {
                Debug.Log("hit");
                hit();
            }
        }
    }
    public void Attack() => coroutine ??= StartCoroutine(Attacking());
    public void Block() => coroutine ??= StartCoroutine(Blocking());
    public void Dodge(Vector2 DodgeInput)
    {
        if (dodgeCooldown && !isDodging && !isFall)
        {
            isAttacking = false;
            isBlocking = false;
            StopAllCoroutines();

            StartCoroutine(Dodging(DodgeInput));
        }
    }

    #region Attack
    public IEnumerator Attacking()
    { 
        if (Time.time >= time || index + 1 >= TotalCombos) index = 0;
        else index++;

        isAttacking = true;

        BasicAttackInfo info = attackInfos[index];
        animationName = "Attack" + (index + 1).ToString();
        if (animationName == "Attack1") animator.Play(animationName);
        else animator.CrossFade(animationName, 0.25f);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        ResetHitEnemies();
        min_frameTime = info.min_frameTime / info.MaxFrames;
        max_frameTime = info.max_frameTime / info.MaxFrames;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);

        isAttacking = false;
        time = Time.time + ComboInterval;
        coroutine = null;
    }
    public void hit()
    {
        Collider[] hitenemy = Physics.OverlapSphere(attackPoint.position, 0.5f, enemyLayers);
        foreach (Collider col in hitenemy)
        {
            if (col.gameObject.TryGetComponent(out IEntity enemy) && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                enemy.ReceiveDamage(70);
                _player.HealthPlayer += 20f;
                _player.TempHealth += 20f;
            }
        }
    }
    public void ResetHitEnemies() => hitEnemies.Clear();
    #endregion
    public IEnumerator Dodging(Vector2 DodgeInput)
    {
        isDodging = true;
        animator.SetTrigger("Dodge");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"));
        float dodgeDuration = 0.7f;
        float elapsedTime = 0f;
        while (elapsedTime < dodgeDuration)
        {
            elapsedTime += Time.deltaTime;
            IPmovement.Dash(DodgeInput);
            yield return null;
        }
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);
        isDodging = false;
        dodgeTime = Time.time + DodgeCooltime;

        coroutine = null;
    }

    public IEnumerator Blocking()
    {
        if (isBlocking == false && _player._CameraStyle == CameraStyle.Combat)
        {
            Debug.Log("Block");
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
    public void OnHit()
    {
        if (!isFall)
        {
            isAttacking = false;
            StopAllCoroutines();
            coroutine = StartCoroutine(Interrupt());
        }
    }
    public IEnumerator Interrupt()
    {
        isFall = true;
        animator.SetTrigger("Fall");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Fall and Stand"));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f);

        coroutine = null;
        isFall = false;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
    }
#endif
}
