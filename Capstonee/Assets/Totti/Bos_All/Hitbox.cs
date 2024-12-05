using System.Collections.Generic;
using UnityEngine;
public class Hitbox : TimedObject
{
    [Header("Hitbox Settings")]
    [SerializeField] private float Damage;
    [SerializeField] private float radius;
    [SerializeField] private float attackDuration;
    public LayerMask AttackLayer;

    private List<Collider> hits = new();
    private float time_elapsed;
    private ParticleSystem particle;
    private void Awake()
    {
        if(TryGetComponent(out particle))
        {
            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
    }
    private void OnEnable()
    {
        hits.Clear();
        time_elapsed = 0;
    }
    protected override void Update()
    {
        base.Update();
        //Kode tidak akan jalan kalo stop harusnya;
        time_elapsed += Time.deltaTime;
        if (time_elapsed <= attackDuration)
        {
            Collider[] hitTargets = Physics.OverlapSphere(transform.position, radius, AttackLayer);

            foreach (Collider target in hitTargets)
            {
                // Pastikan target belum terkena damage
                if (!hits.Contains(target))
                {
                    if (target.TryGetComponent<IEntity>(out var entity))
                    {
                        entity.ReceiveDamage(Damage);
                        hits.Add(target); // Tambahkan target ke set setelah diberi damage
                    }
                }
            }
        }
    }
    public override void OnContinue()
    {
        if (particle != null) particle.Play();
    }
    public override void OnStop()
    {
        if (particle != null) particle.Pause();
    }
    private void OnParticleSystemStopped()
    {
        PoolManager.ReleaseObject(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
