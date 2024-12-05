using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct hitBoxInfo
{
    [Header("Hitbox Settings")]
    public GameObject HitBoxPrefabs;
    public Transform HitBox;

    [Header("Frame Settings")]
    public float SpawnFrame;
}
[Serializable]
public struct TriggerInfo
{
    public string TriggerName;
    public string ANIMATIONNAME;
    public List<hitBoxInfo> hitboxesInfo;
    public float TotalFrames;

}
public class BossCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange;
    public float attackDelay;
    public float attackDelayAfter;
    private float lastAttackTime = 0f;
    private int lastAttackIndex;
    public Transform player;
    private int Damage;
    public int MawForce;

    private Animator animator;
    public bool isAttack = false;
    private BossBehaviour bossBehaviour;

    [Header("Attack Settings")]
    public List<GameObject> hitboxPrefabs = new List<GameObject>();
    public Transform[] attackPoints;

    public LayerMask targetLayer;
    private bool SFXPlaying;

    [SerializeField] private List<TriggerInfo> infos;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bossBehaviour = GetComponent<BossBehaviour>();
        attackDelay = bossBehaviour.CD;
        Damage = bossBehaviour.ATK;
        //lastAttackTrigger = "";

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }
    private void Update()
    {
        if (bossBehaviour.isAlive)
        {
            if (!isAttack && !SoundManager.instance.IsSFXPlaying("BossStep"))
            {
                SoundManager.instance.PlaySFX("BossStep");
            }
            else if (isAttack && SoundManager.instance.IsSFXPlaying("BossStep"))
            {
                SoundManager.instance.StopSFX("BossStep");
            }
        }

        if (bossBehaviour.CurrTP > 0 && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttack && Time.time >= lastAttackTime + attackDelay)
            {
                //Debug.Log("ketemu");
                StartCoroutine(AttackPlayer());
                lastAttackTime = Time.time;
            }
        }
        else if (bossBehaviour.CurrTP <= 0)
        {
            Die();
        }
    }

    private IEnumerator AttackPlayer()
    {
        //Debug.Log("SiapPukul");
        isAttack = true;

        bossBehaviour.agent.ResetPath();
        bossBehaviour.agent.isStopped = true;
        bossBehaviour.agent.speed = 0;

        var Rtemp = bossBehaviour.RotateSpeed;
        bossBehaviour.RotateSpeed = 0;

        int randomAttack;
        int count = infos.Count;

        do
        {
            randomAttack = UnityEngine.Random.Range(1, count);
        } 
        while (randomAttack == lastAttackIndex);

        lastAttackIndex = randomAttack;

        yield return PlayAnimationYA(infos[randomAttack], infos[randomAttack].TotalFrames);

        yield return new WaitForSeconds(attackDelayAfter);

        isAttack = false;
        bossBehaviour.agent.isStopped = false;
        bossBehaviour.agent.speed = bossBehaviour.SPD;
        bossBehaviour.RotateSpeed = Rtemp;
    }
    private void Die()
    {
        animator.enabled = false;
        animator.enabled = true;
        animator.SetTrigger("Dying");
        SoundManager.instance.PlaySFX("Dying");
        StopAllCoroutines();
        bossBehaviour.isAlive = false;
        this.enabled = false;
    }
    IEnumerator PlayAnimationYA(TriggerInfo info, float TotalFrames)
    {
        animator.SetTrigger(info.TriggerName);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(info.ANIMATIONNAME));

        foreach (hitBoxInfo trigger in info.hitboxesInfo)
        {
            float NormalizedTime = trigger.SpawnFrame/TotalFrames;

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > NormalizedTime);

            SpawnHitbox(trigger);
        }
    }
    private void SpawnHitbox(hitBoxInfo info)
    {
        var obj = PoolManager.GetObject(info.HitBoxPrefabs, false);
        obj.transform.position = info.HitBox.position;
        obj.transform.forward = transform.forward;
        obj.SetActive(true);
    }
    private IEnumerator GoToPlayer()
    {
        yield return new WaitForSeconds(attackDelay);

        if (player != null)
        {
            float duration = 0.4f;
            float elapsedTime = 0f;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = new Vector3(player.position.x, startPosition.y, player.position.z);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                yield return null;
            }

            transform.position = targetPosition;
            bossBehaviour.RotateToTarget(0);
        }
        else
        {
            //Debug.LogWarning("Player not found, cannot move.");
        }
    }

    public void Roars()
    {
        SoundManager.instance.PlaySFX("Roar");
    }
    public void Fissures()
    {
        SoundManager.instance.PlaySFX("Fissure");
    }
}
