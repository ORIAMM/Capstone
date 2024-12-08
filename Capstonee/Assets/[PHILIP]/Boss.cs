using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : TimedObject, IEntity
{
    [Header("Enemy Stats")]
    [SerializeField] private float MaxHealth;
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float Offset;
    [SerializeField] private float RotateSpeed;
    [SerializeField] private float DampingValue;
    [SerializeField] private float gravity;
    [SerializeField] private float increasedSpeed = 1;

    [Header("Target Settings")]
    [SerializeField] private float TargetRadius;
    [SerializeField] private LayerMask PlayerLayer;

    [Header("Attack Settings")]
    [SerializeField] private List<EnemyAttackInfo> AttackInfos = new();
    [SerializeField] private List<AttackPattern> AttackPatterns = new();

    [Header("UI Settings")]
    [SerializeField] Image BossHPBar;

    private float Health;
    private float _health
    {
        get
        {
            return Health;
        }
        set
        {
            Health = value;
            if (Health <= 0) OnDeath();
            else if (BossHPBar) BossHPBar.fillAmount = Health / MaxHealth;
        }
    }
    private Transform target;

    private Vector3 moveDirection, keptVelocity;
    private int patternCount, atkinfoCount, patternNextLine;
    private float atkRadius, atkCooldown;
    private float keptAnimationSpeed;

    private EnemyAttackInfo currentAttackInfo;
    private AttackPattern pattern;

    private Coroutine isAttacking;
    private CharacterController controller;
    private Animator animator;

    float rotspeed = 0, mainrotspeed, toMove;
    float endLerp = 0;
    private void Awake()
    {
        _health = MaxHealth;
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        patternCount = AttackPatterns.Count;
        atkinfoCount = AttackInfos.Count;
    }
    private void Start()
    {
        currentAttackInfo = null;
        mainrotspeed = RotateSpeed;
    }
    public override void OnUpdate()
    {
        currentAttackInfo ??= QueueNextAttack();

        if (rotspeed != 0) LookToPlayer(0, rotspeed);
        if (target)
        {
            if (toMove != 0)
            {
                float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (time > endLerp)
                {
                    toMove = 0;
                    moveDirection = new Vector3(0, moveDirection.y, 0);
                }
                else
                {
                    moveDirection = toMove * transform.forward;
                }
            }
            else if (currentAttackInfo != null && isInRange(atkRadius) && Time.time > atkCooldown)
            {
                isAttacking ??= StartCoroutine(PlayAnimation());
            }
            else if (isAttacking == null)
            {
                LookToPlayer(Offset, mainrotspeed);
                GetForwardMove();
            }
        }
        ApplyMovement(toMove == 0);
    }
    public bool isInRange(float radius)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        return distanceToPlayer <= radius;
    }

    public EnemyAttackInfo QueueNextAttack()
    {
        Debug.Log("DODODOOD");
        Target();
        if(target != null)
        {
            EnemyAttackInfo info;
            pattern ??= GetPattern();
            if (pattern == null) info = AttackInfos[UnityEngine.Random.Range(0, atkinfoCount)];
            else
            {
                info = AttackInfos.Find(x => x.attack_Type == pattern.attack_Types[patternNextLine]);
                patternNextLine = (patternNextLine + 1) % pattern.attack_Types.Count;
                if (patternNextLine == 0) pattern = null;
            }
            atkRadius = info.AttackRadius;
            return info;
        }
        return null;
    }
    public AttackPattern GetPattern()
    {
        int count = AttackPatterns.Count;
        int random = UnityEngine.Random.Range(0, count + 1);
        if (random == count) return null;
        patternNextLine = 0;
        return AttackPatterns[random];
    }
    public override void OnContinue()
    {
        if (animator)
        {
            animator.speed = keptAnimationSpeed;
            controller.Move(keptVelocity);
            keptVelocity = Vector3.zero;
        }
    }
    public override void OnStop()
    {
        if (animator)
        {
            keptAnimationSpeed = animator.speed;
            animator.speed = 0;
            keptVelocity = controller.velocity;
            controller.Move(Vector3.zero);
        }
    }
    public void ReceiveDamage(float value)
    {
        _health -= (int)value;
    }
    public void OnDeath()
    {
        animator.SetTrigger("Dying");
        SoundManager.instance.PlaySFX("Dying");
        UI_Controller.instance.isWin = true;
    }
    #region Movement, Target, rotation
    public void GetForwardMove()
    {
        var forward = movementSpeed * transform.forward;
        moveDirection += new Vector3(forward.x, 0, forward.z);
    }
    public void ApplyMovement(bool dontUseClamping)
    {
        float yMagnitude = moveDirection.y;
        if (dontUseClamping)
        {
            moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
            moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), movementSpeed) + Vector3.up * yMagnitude;
        }
        //mainrotspeed = Mathf.Lerp(mainrotspeed, 0, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude / movementSpeed);
        controller.Move(moveDirection * Time.deltaTime);
        moveDirection.y -= gravity * Time.deltaTime;
    }
    public void LookToPlayer(float offset, float rotateSpeed)
    {
        Vector3 directionToPlayer = target.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        Quaternion offsetRot = Quaternion.Euler(0, offset, 0);
        targetRotation *= offsetRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void Target()
    {
        float closestDistance = TargetRadius + 1;
        Collider player = null;
        List<Collider> players = Physics.OverlapSphere(transform.position, TargetRadius, PlayerLayer).ToList();
        foreach (Collider ply in players)
        {
            float distance = Vector3.Distance(ply.transform.position, transform.position);
            if (closestDistance > distance)
            {
                player = ply;
                closestDistance = distance;
            }
        }
        if (!player) return;
        else target = player.transform;
    }
    #endregion
    #region AtkPatterns
    public IEnumerator PlayAnimation()
    {
        Debug.Log("ANIMATIONPLAYING");
        int TotalFrames = currentAttackInfo.info.TotalFrames;
        var objectInfo = currentAttackInfo.info.objectInfos;
        var moveInfo = currentAttackInfo.info.moveInfos;
        int objectIndex = 0, maxobjectIndex = objectInfo.Count;
        int moveIndex = 0, maxmoveIndex = moveInfo.Count;
        Vector3 toKeep = moveDirection;
        moveDirection = Vector3.zero;
        animator.SetTrigger(currentAttackInfo.info.TriggerName);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(currentAttackInfo.info.ANIMATIONNAME));
        while (maxobjectIndex > objectIndex || maxmoveIndex > moveIndex)
        {
            if (moveIndex != maxmoveIndex && moveInfo[moveIndex].SpawnFrame < objectInfo[objectIndex].SpawnFrame)
            {
                EnemyAttackInfo.MoveInfo move = moveInfo[moveIndex];
                float NormalizedTime = (float)move.SpawnFrame / TotalFrames;
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > NormalizedTime);
                animator.speed = move.animation_speed == 0 ? 1 : move.animation_speed;
                rotspeed = move.rotation_speed;
                if (move.speed != 0)
                {
                    Vector3 playerpos = isInRange(move.range) ? target.position * increasedSpeed : transform.position + transform.forward * move.speed;
                    //playerpos = transform.forward * move.speed;
                    playerpos = playerpos - transform.position;
                    endLerp = move.lerpEndFrame / TotalFrames;
                    toMove = playerpos.magnitude;
                }
                moveIndex++;
            }
            else
            {
                EnemyAttackInfo.ObjectInfo obj = objectInfo[objectIndex];
                float NormalizedTime = (float)obj.SpawnFrame / TotalFrames;
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > NormalizedTime);
                Debug.Log($"{animator.GetCurrentAnimatorStateInfo(0).normalizedTime}, {NormalizedTime}");
                SpawnHitbox(obj);
                objectIndex++;
            }
        }
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        atkCooldown = Time.time + currentAttackInfo.delay;

        animator.speed = 1;
        rotspeed = 0;
        moveDirection = toKeep;
        isAttacking = null;
            Debug.Log("ANIMATIONFINISHED");
        currentAttackInfo = null;
    }
    private void SpawnHitbox(EnemyAttackInfo.ObjectInfo info)
    {
        var obj = PoolManager.GetObject(info.Prefab, false);
        obj.transform.position = info.spawn_point.position;
        obj.transform.forward = transform.forward;
        obj.SetActive(true);
    }
    #endregion
#if UNITY_EDITOR
    [SerializeField] private float range;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
