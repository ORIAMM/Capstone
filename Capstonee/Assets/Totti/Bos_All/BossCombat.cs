using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange;
    private float attackDelay;
    private float lastAttackTime = 0f;
    private string lastAttackTrigger;
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

    private Dictionary<string, float> attackDurations = new Dictionary<string, float>()
    {
        { "trigger1", 4.2f },    // Charge Pounce
        { "trigger2", 5f },    // Claw Slash
        { "trigger3", 2.3f },  // Fissure
        { "trigger4", 3.2f },  // Pounce
        { "trigger5", 1.2f }   // Kick
    };

    private void Start()
    {
        animator = GetComponent<Animator>();
        bossBehaviour = GetComponent<BossBehaviour>();
        attackDelay = bossBehaviour.CD;
        Damage = bossBehaviour.ATK;
        lastAttackTrigger = "";

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
                Debug.Log("Musik");
                SoundManager.instance.PlaySFX("BossStep");
            }
            else if (isAttack && SoundManager.instance.IsSFXPlaying("BossStep"))
            {
                Debug.Log("Berhenti");
                SoundManager.instance.StopSFX("BossStep");
            }
        }

        if (bossBehaviour.CurrTP > 0 && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttack && Time.time >= lastAttackTime + attackDelay)
            {
                Debug.Log("ketemu");
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
        Debug.Log("SiapPukul");
        isAttack = true;

        bossBehaviour.agent.ResetPath();
        bossBehaviour.agent.isStopped = true;
        bossBehaviour.agent.speed = 0;

        var Rtemp = bossBehaviour.RotateSpeed;
        bossBehaviour.RotateSpeed = 0;

        int randomAttack;
        string attackTrigger;

        do
        {
            randomAttack = Random.Range(1, 6);
            attackTrigger = $"trigger{randomAttack}";
        } 
        while (attackTrigger == lastAttackTrigger);

        lastAttackTrigger = attackTrigger;

        animator.SetTrigger(attackTrigger);
        Debug.Log(attackTrigger);
        float animationLength = GetAttackAnimationDuration(attackTrigger);
        AttackFunction(attackTrigger);

        yield return new WaitForSeconds(animationLength);

        isAttack = false;
        bossBehaviour.agent.isStopped = false;
        bossBehaviour.agent.speed = bossBehaviour.SPD;
        bossBehaviour.RotateSpeed = Rtemp;
    }

    private float GetAttackAnimationDuration(string attackTrigger)
    {
        if (attackDurations.ContainsKey(attackTrigger))
        {
            return attackDurations[attackTrigger];
        }
        else
        {
            // Kembalikan nilai default jika trigger tidak ditemukan
            return 1f;  // Misalnya, durasi default 1 detik
        }
    }

    private void Die()
    {
        animator.enabled = false;
        animator.enabled = true;
        animator.SetTrigger("Dying");
        StopAllCoroutines();
        bossBehaviour.isAlive = false;
        this.enabled = false;
    }
    void AttackFunction(string attacktrigger)
    {

        if (attacktrigger == "trigger1")
        {
            SoundManager.instance.PlaySFX("ChargePounch");
            StartCoroutine(GoToPlayer());
            StartCoroutine(HandleAttackWithDelay(1, 2.6f, 0));
        }
        else if (attacktrigger == "trigger2")
        {
            SoundManager.instance.PlaySFX("Claws");
            StartCoroutine(HandleAttackWithDelay(1, 1.5f, 1));
            StartCoroutine(HandleAttackWithDelay(0, 2.8f, 1));
            StartCoroutine(HandleAttackWithDelay(1, 4.9f, 2));
            SoundManager.instance.PlaySFX("Roar");
        }
        else if (attacktrigger == "trigger3")
        {
            SoundManager.instance.PlaySFX("Roar");
            StartCoroutine(HandleAttackWithDelay(1, 2.2f, 3));
            SoundManager.instance.PlaySFX("Fissure");
        }
        else if (attacktrigger == "trigger4")
        {
            StartCoroutine(GoToPlayer());
            StartCoroutine(HandleAttackWithDelay(1, 1.7f, 4));
            SoundManager.instance.PlaySFX("Pounch");
        }
        else if (attacktrigger == "trigger5")
        {
            SoundManager.instance.PlaySFX("Kick");
            StartCoroutine(HandleAttackWithDelay(2, 0.7f, 5));
        }
    }
    IEnumerator HandleAttackWithDelay(int hitboxType, float delay, int hitboxIndex)
    {
        yield return new WaitForSeconds(delay);
        SpawnHitboxAt(hitboxType, hitboxIndex);
    }

    private void SpawnHitboxAt(int attackPointIndex, int prefabIndex)
    {
        if (attackPointIndex < attackPoints.Length && hitboxPrefabs != null)
        {
            Transform spawnPoint = attackPoints[attackPointIndex];
            GameObject hitbox = Instantiate(hitboxPrefabs[prefabIndex], spawnPoint.position, spawnPoint.rotation);
            BossAttackHitbox attackHitbox = hitbox.GetComponent<BossAttackHitbox>();

            if (attackHitbox != null)
            {
                attackHitbox.Initialize(bossBehaviour);
            }
        }
    }

    //Ini teleport
    //private IEnumerator GoToPlayer()
    //{

    //    yield return new WaitForSeconds(attackDelay);

    //    if (player != null)
    //    {
    //        transform.position = player.position;
    //    }
    //    else
    //    {
    //        //Debug.LogWarning("Player not found, teleportation failed.");
    //    }
    //}

    //Ini Physics make lerp untuk interpolate, bukan teleport
    private IEnumerator GoToPlayer()
    {
        yield return new WaitForSeconds(attackDelay);

        if (player != null)
        {
            float duration = 0.5f;
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
        }
        else
        {
            Debug.LogWarning("Player not found, cannot move.");
        }
    }



}
