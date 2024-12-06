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
    public bool isStopped;
    private int tempRotateSpeed;

    public bool isAlive;
    private bool isPlaySFX;
    private TimeManager timeManager;
    private Animator animator;

    private void Start()
    {
        isStopped = false;
        agent = GetComponent<NavMeshAgent>();
        bossCombat = GetComponent<BossCombat>();
        agent.speed = SPD;
        CurrTP = MaxTP;
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

        if (ui.isLose)
        {
            isStopped = true;
        }
    }
    public override void OnStop()
    {
        if (animator != null)
        {
            isStopped = true;
            animator.speed = 0; // Freeze the animation
            RotateSpeed = 0;
            agent.ResetPath();
            agent.isStopped = true;
            agent.speed = 0;
            SoundManager.instance.PauseSfx();
        }
    }
    public override void OnContinue()
    {
        if (animator != null)
        {
            agent.speed = SPD;
            animator.speed = 1; // Resume the Animator
            RotateSpeed = tempRotateSpeed;
            agent.isStopped = false;
            isStopped = false;
            SoundManager.instance.UnPauseSfx();
        }
    }
    void ChasePlayer()
    {
        FindPlayer();

        if (player != null && !bossCombat.isAttack && !isStopped)
        {
            agent.SetDestination(player.position);
            
        }
        
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

        if (!isStopped)
        {
            // Hitung rotasi target dengan offset pada sumbu Y
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion offsetRot = Quaternion.Euler(0, offsetY, 0);
            targetRot *= offsetRot;

            // Rotasikan objek menuju target dengan offset
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotateSpeed);
        }

    }
    //public void RotateToTarget(float offsetY)
    //{
    //    // Hitung arah ke target
    //    Vector3 dir = player.position - transform.position;
    //    dir.y = 0;

    //    // Hitung jarak ke target
    //    float distanceToTarget = dir.magnitude;

    //    // Tentukan jarak minimum untuk melakukan rotasi
    //    float minDistance = -1f; // Sesuaikan nilai ini dengan kebutuhan

    //    if (!isStopped && distanceToTarget > minDistance)
    //    {
    //        // Hitung rotasi target dengan offset pada sumbu Y
    //        Quaternion targetRot = Quaternion.LookRotation(dir);
    //        Quaternion offsetRot = Quaternion.Euler(0, offsetY, 0);
    //        targetRot *= offsetRot;

    //        // Rotasikan objek menuju target dengan offset
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
    //    }
    //}


    public UI_Controller ui;
    void BossDeath()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

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

        if (CurrTP <= 0)
        {
            CurrTP = 0;
            isAlive = false;
            OnDeath();
        }
    }
    public void OnDeath()
    {
        SoundManager.instance.StopAllMusic();
        SoundManager.instance.StopAllSFX();
        ui.isWin = true;
        ui.Victory();
        BossDeath();
    }
}
