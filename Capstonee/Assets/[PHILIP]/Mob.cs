using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//public abstract class AttackInfo : MonoBehaviour
//{
//    public float distance;
//    public float delay;
//    public Animator animator;

//    public abstract IEnumerator AttackBehaviour();
//    public IEnumerator PlayAnimation(TriggerInfo info, float TotalFrames)
//    {
//        animator.SetTrigger(info.TriggerName);

//        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(info.ANIMATIONNAME));

//        foreach (hitBoxInfo trigger in info.hitboxesInfo)
//        {
//            float NormalizedTime = trigger.SpawnFrame / TotalFrames;

//            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > NormalizedTime);

//            SpawnHitbox(trigger);
//        }
//    }
//    private void SpawnHitbox(hitBoxInfo info)
//    {
//        var obj = PoolManager.GetObject(info.HitBoxPrefabs, false);
//        obj.transform.position = info.HitBox.position;
//        obj.transform.forward = transform.forward;
//        obj.SetActive(true);
//    }
//}
//public class Mob_BasicAttack : AttackInfo
//{
//    public TriggerInfo info;

//    public Mob_BasicAttack(float distance, Transform transform, Animator animator, TriggerInfo info) : base(distance, transform, animator)
//    {
//        this.info = info;
//    }
//    public override IEnumerator AttackBehaviour()
//    {
//        yield return PlayAnimation(info, info.TotalFrames);
//    }
//}
public class Mob : TimedObject, IEntity
{
    [SerializeField] private float Health;
    [SerializeField] private float AttackRadius;
    [SerializeField] private float AttackDelay;
    [SerializeField] private TriggerInfo info;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float ChaseRadius;
    [SerializeField] private float DampingValue;

    [SerializeField] private LayerMask playerLayer;
    //private List<AttackInfo> attackInfos = new();
    private CharacterController controller;
    private Animator animator;

    private float initial_Health;
    private Transform player;


    Vector3 keptVelocity;
    float atkCooldown;
    Vector3 moveDirection = Vector3.zero;
    Coroutine isAttacking;
    //private AttackInfo currentAttackQueue;
    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        initial_Health = Health;
    }
    public override void OnUpdate()
    {
        Target();
        if (player && isAttacking == null)
        {
            LookToPlayer();
            GetForwardMove();
        }
        ApplyMovement();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= AttackRadius && Time.time > atkCooldown)
        {
            isAttacking = StartCoroutine(PlayAnimation(info, info.TotalFrames));
        }
    }
    public void LookToPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void GetForwardMove()
    {
        var forward = movementSpeed * transform.forward;
        moveDirection += new Vector3(forward.x, 0, forward.z);
    }
    public void Target()
    {
        float closestDistance = ChaseRadius + 1;
        Collider player = null;
        List<Collider> players = Physics.OverlapSphere(transform.position, ChaseRadius, playerLayer).ToList();
        foreach(Collider ply in players)
        {
            float distance = Vector3.Distance(ply.transform.position, transform.position);
            if (closestDistance > distance)
            {
                player = ply;
                closestDistance = distance;
            }
        }
        if (!player) return;
        else this.player = player.transform;
    }
    public void ApplyMovement()
    {
        float yMagnitude = moveDirection.y;
        moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
        moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), movementSpeed) + Vector3.up * yMagnitude;
        controller.Move(moveDirection * Time.deltaTime);
        Vector3 velValue = new(controller.velocity.x, 0, controller.velocity.z);
        Vector3 forwardVel = new(transform.forward.x, 0, transform.forward.z);
        Vector3 rightVel = new(transform.right.x, 0, transform.right.z);
        moveDirection.y -= gravity * Time.deltaTime;

        if (velValue.magnitude > 0.01f)
        {
            animator.SetFloat("VelX", Vector3.Dot(rightVel, velValue) / movementSpeed);
            animator.SetFloat("VelY", Vector3.Dot(forwardVel, velValue) / movementSpeed);
        }
    }
    public override void OnContinue()
    {
        if (animator)
        {
            animator.speed = 1;
            controller.Move(keptVelocity);
            keptVelocity = Vector3.zero;
            Debug.Log("Continue");
        }
    }
    public override void OnStop()
    {
        if (animator)
        {
            animator.speed = 1;
            keptVelocity = controller.velocity;
            controller.Move(Vector3.zero);
            Debug.Log("Continue");
        }
    }
    public void OnDeath()
    {
        PoolManager.ReleaseObject(gameObject);
    }
    public void ReceiveDamage(float value)
    {
        initial_Health -= (int)value;

        if (initial_Health <= 0)
        {
            initial_Health = 0;
            OnDeath();
        }
    }
    public IEnumerator PlayAnimation(TriggerInfo info, float TotalFrames)
    {
        animator.SetTrigger(info.TriggerName);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(info.ANIMATIONNAME));

        foreach (hitBoxInfo trigger in info.hitboxesInfo)
        {
            float NormalizedTime = trigger.SpawnFrame / TotalFrames;

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > NormalizedTime);

            SpawnHitbox(trigger);
        }
        isAttacking = null;
        atkCooldown = Time.time + AttackDelay;
    }
    private void SpawnHitbox(hitBoxInfo info)
    {
        var obj = PoolManager.GetObject(info.HitBoxPrefabs, false);
        obj.transform.position = info.HitBox.position;
        obj.transform.forward = transform.forward;
        obj.SetActive(true);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ChaseRadius);
    }


#endif
}
