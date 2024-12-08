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
    [Header("Combat")]
    [SerializeField] int TotalCombos = 0;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float DodgeCooltime;
    public float ComboInterval = 0.3f;

    [Header("Reference")]
    [HideInInspector] public Animator animator;
    [SerializeField] private LayerMask EnemyL;

    //Reference
    private ImprovisedPlayerMovement IPmovement;
    private PlayerCamera PlayerCamera;
    private HashSet<BossBehaviour> hitEnemies = new HashSet<BossBehaviour>();
    private Player _player;

    [HideInInspector] public Transform player;

    public bool isAttacking 
    {  
        get ; 
        private set; 
    }
    public bool isBlocking
    {
        get;
        private set;
    }
    public bool isDodging
    {
        get;
        private set;
    }
    public bool isFall
    {
        get;
        private set;
    }
    public Coroutine coroutine { get; private set; }
    float time = 0;
    int index = 0;
    float dodgeTime = 0;
    bool dodgeCooldown => Time.time >= dodgeTime;

    private void Start()
    {
        IPmovement = GetComponent<ImprovisedPlayerMovement>();
        _player = GetComponent<Player>();
    }
    public void Attack() => coroutine ??= StartCoroutine(Attacking());
    public void Block() => coroutine ??= StartCoroutine(Blocking());
    public void Dodge(Vector2 DodgeInput) => coroutine ??= StartCoroutine(Dodging(DodgeInput));

    public void Interupt() => StartCoroutine(Interupting());

    public IEnumerator Attacking()
    {
        if (_player._CameraStyle == CameraStyle.Combat)
        {
            if (Time.time >= time || index + 1 >= TotalCombos) index = 0;
            else index++;

            isAttacking = true;
            Debug.Log(index);

            yield return null;
            //float ProjectileSpawnTime = ProjectileTimeline[index].type == Attack_Type.projectile ? ProjectileTimeline[index].FrameInWhichProjectileSpawn / ProjectileTimeline[index].AnimationFrames : 10;
            string animationName = "Attack" + (index + 1).ToString();

            if (animationName == "Attack1") animator.Play(animationName);
            else animator.CrossFade(animationName, 0.25f);

            //animator.Play(animationName);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f);

            isAttacking = false;
            time = Time.time + ComboInterval;
            coroutine = null;
        }
        
    }
    
    public IEnumerator Blocking()
    {
        if (isBlocking == false && _player._CameraStyle == CameraStyle.Combat)
        {
            Debug.Log("Bloc");
            isBlocking = true;
            animator.SetBool("isBlock", isBlocking);
            yield return new WaitForSeconds(2f);

            isBlocking = false;
            animator.SetBool("isBlock", isBlocking);
        }
        coroutine = null;
    }

    public IEnumerator Dodging(Vector2 DodgeInput)
    {
        if (isDodging == false && dodgeCooldown)
        {
            Debug.DrawRay(player.position, player.forward * 5f, Color.red, 1f);
            dodgeTime = Time.time + DodgeCooltime;
            isDodging = true;
            animator.SetTrigger("Dodge");
            switch (_player._CameraStyle)
            {
                case CameraStyle.Combat:
                    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge0"));
                    float combatDodgeDuration = 0.7f;
                    float elapsedCombatTime = 0f;

                    /*Vector3 dashDirection = player.forward;
                    if (DodgeInput.magnitude > 0)
                    {
                        Vector3 inputDirection = new Vector3(DodgeInput.x, 0, DodgeInput.y).normalized;
                        dashDirection = player.TransformDirection(inputDirection);
                    }*/
                    while (elapsedCombatTime < combatDodgeDuration)
                    {
                        elapsedCombatTime += Time.deltaTime;
                        IPmovement.Dash();
                        IPmovement.ApplyMove(0);
                        yield return null;
                    }
                    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);
                    isDodging = false;
                    coroutine = null;

                    break;

                case CameraStyle.Basic:
                    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"));
                    float dodgeDuration = 0.7f;
                    float elapsedTime = 0f;
                    while (elapsedTime < dodgeDuration)
                    {
                        elapsedTime += Time.deltaTime;
                        IPmovement.Dash();
                        IPmovement.ApplyMove(0);
                        yield return null;
                    }
                    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);
                    isDodging = false;
                    coroutine = null;
                    break;
            }

        }
    }

    public void hit()
    {
        bool isHit = Physics.CheckSphere(attackPoint.position, 0.5f, EnemyL);
        if (isHit)
        {
            Collider[] hitenemy = Physics.OverlapSphere(attackPoint.position, 0.5f, EnemyL);
            foreach (Collider col in hitenemy)
            {
                BossBehaviour enemy = col.gameObject.GetComponent<BossBehaviour>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    hitEnemies.Add(enemy);
                    enemy.ReceiveDamage(70);
                    _player.HealthPlayer += 20f;
                }
            }
        }
    }
    public void ResetHitEnemies()
    {
        hitEnemies.Clear(); // Clear the HashSet at the end of the animation
    }
    public void Impact()
    {
        StopCoroutine(Blocking());
        animator.SetTrigger("Impact");
        isBlocking = false;
        coroutine = null;
    }
    public IEnumerator Interupting()
    {
        Debug.Log("Interrupt");
        if (isFall == false)
        {
            isFall = true;
            StopCoroutine(Attacking());
            animator.SetTrigger("Fall");
            yield return new WaitForSeconds(1f);

            coroutine = null;
            isAttacking = false;
            isFall = false;
        }
        
    }
}
