using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange;
    private float attackDelay;
    private float lastAttackTime = 0f;
    public Transform player;

    private Animator animator;
    public bool isAttack = false;
    private BossBehaviour bossBehaviour;

    private Dictionary<string, float> attackDurations = new Dictionary<string, float>()
    {
        { "trigger1", 3f },    // Charge Pounce
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

        int randomAttack = Random.Range(1, 6);
        string attackTrigger = $"trigger{randomAttack}";
        animator.SetTrigger(attackTrigger);
        Debug.Log(attackTrigger);
        float animationLength = GetAttackAnimationDuration(attackTrigger);

        yield return new WaitForSeconds(animationLength);

        isAttack = false;
        bossBehaviour.agent.isStopped = false;
        bossBehaviour.agent.speed = bossBehaviour.SPD;
    }

    private float GetAttackAnimationDuration(string attackTrigger)
    {
        // Memeriksa apakah trigger ada di dalam dictionary dan mengambil durasinya
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
}
