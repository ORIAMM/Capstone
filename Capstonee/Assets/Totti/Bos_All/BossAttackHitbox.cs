using System.Collections.Generic;
using UnityEngine;
public class BossAttackHitbox : TimedObject
{
    [Header("Attack Settings")]
    public float damage;
    public float StartDuration;
    public float duration;
    public float range;
    public LayerMask targetLayer;

    private new ParticleSystem particleSystem;
    private Coroutine damageCoroutine;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Initialize(BossBehaviour boss)
    {
        damage = boss.ATK;
        StartDuration = boss.SkillDuration;
        duration = StartDuration;
        range = boss.AtkRange;

        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        damageCoroutine = StartCoroutine(DealDamageWithSphere());
    }

    public override void OnUpdate()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            PoolManager.ReleaseObject(gameObject);
            duration = StartDuration;
        }
    }

    public override void OnContinue()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DealDamageWithSphere());
        }
    }

    public override void OnStop()
    {
        if (particleSystem != null)
        {
            particleSystem.Pause();
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnParticleSystemStopped()
    {
        PoolManager.ReleaseObject(gameObject);
    }

    private System.Collections.IEnumerator DealDamageWithSphere()
    {
        float elapsedTime = 0f;
        HashSet<Collider> hitTargetsSet = new HashSet<Collider>(); // Set untuk melacak target yang sudah terkena damage

        while (elapsedTime < duration)
        {
            if (TimeManager.instance.isStopped)
            {
                yield return null; // Tunggu sampai waktu berjalan kembali
                continue;
            }

            elapsedTime += Time.deltaTime;

            Collider[] hitTargets = Physics.OverlapSphere(transform.position, range, targetLayer);

            foreach (Collider target in hitTargets)
            {
                // Pastikan target belum terkena damage
                if (!hitTargetsSet.Contains(target))
                {
                    IEntity entity = target.GetComponent<IEntity>();
                    if (entity != null)
                    {
                        entity.ReceiveDamage(damage);
                        hitTargetsSet.Add(target); // Tambahkan target ke set setelah diberi damage
                    }
                }
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, range);
    }
}
