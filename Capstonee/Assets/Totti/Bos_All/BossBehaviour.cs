using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossBehaviour : TimedObject, IEntity
{
    [Header("Object Reference")]
    public Transform player;
    public NavMeshAgent agent;
    public Image BossHPBar;
    public BossCombat bossCombat;

    [Header("Value")]
    public int MaxTP;
    public int CurrTP;
    public int ATK;
    public int DEF;
    public int SPD;
    public float ASPD;
    public float CD;

    public float AtkRange;
    public float SkillDuration;
    public int RotateSpeed;
    public float RotateOffset;

    private int tempRotateSpeed;

    public bool isAlive;
    private bool isPlaySFX;
    private TimeManager timeManager;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bossCombat = GetComponent<BossCombat>();
        agent.speed = SPD;
        isAlive = true;
        isPlaySFX = false;
        animator = GetComponent<Animator>();
        tempRotateSpeed = RotateSpeed;
    }

    public override void OnUpdate()
    {
        UpdateHealthBar();

        if (isAlive)
        {
            ChasePlayer();
        }
        else
        {
            BossDeath();
        }
    }
    public override void OnStop()
    {
        if (animator != null)
        {
            animator.speed = 0; // Freeze the animation
            agent.speed = 0;
            RotateSpeed = 0;

        }
    }


    public override void OnContinue()
    {
        if (animator != null)
        {
            agent.speed = SPD;
            animator.speed = 1; // Resume the Animator
            RotateSpeed = tempRotateSpeed;
        }
    }


    void ChasePlayer()
    {
        FindPlayer();

        if (player != null && !bossCombat.isAttack)
        {
            agent.SetDestination(player.position);
            
            //if (!isPlaySFX)
            //{
            //    Debug.Log("Musik");
            //    SoundManager.instance.PlaySFX("BossStep");
            //    isPlaySFX = true;
            //}
        }
        //else if (bossCombat.isAttack)
        //{
        //    Debug.Log("Berhentii");
        //    SoundManager.instance.StopSFX("BossStep");
        //    isPlaySFX = false;
        //}
        
        RotateToTarget(RotateOffset);
    }
    public void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }
    public void RotateToTarget(float offsetY)
    {
        // Hitung arah ke target
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.magnitude > 0.1f)
        {
            // Hitung rotasi target dengan offset pada sumbu Y
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion offsetRot = Quaternion.Euler(0, offsetY, 0);
            targetRot *= offsetRot;

            // Rotasikan objek menuju target dengan offset
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotateSpeed);
        }
    }
    void BossDeath()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        //Debug.Log("Tururururu");
    }
    void UpdateHealthBar()
    {
        if (BossHPBar != null)
        {
            BossHPBar.fillAmount = Mathf.Clamp01((float)CurrTP / MaxTP);
        }
    }
    public void ReceiveDamage(float value)
    {
        if (!isAlive) return;

        CurrTP -= (int)value - DEF;

        //Debug.Log($"Boss took {value} damage. Current HP: {CurrTP}");

        if (CurrTP <= 0)
        {
            CurrTP = 0;
            isAlive = false;
            OnDeath();
        }
    }
    public void OnDeath()
    {
        BossDeath();
    }


}
