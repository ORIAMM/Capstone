using System.Collections.Generic;
using UnityEngine;

public class Earthquake : TimedObject
{
    [Header("Earthquake Settings")]
    [SerializeField] private float Damage;
    [SerializeField] private float frontOffset;
    [SerializeField] private Vector3 Size;
    [SerializeField] private LayerMask AttackLayer;

    [Header("RaycastDown Settings")]
    [SerializeField] private float particleYOffset;
    [SerializeField] private LayerMask GroundLayer;

    private Vector3 initial_position;
    private float elapsed_time;
    private float lifetime;
    private ParticleSystem particle;
    private List<Collider> hits = new();
    private void Awake()
    {
        if (TryGetComponent(out particle))
        {
            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
            lifetime = main.startLifetime.constantMax;
            Debug.Log(lifetime);
        }
    }
    private void OnEnable()
    {
        Physics.Raycast(transform.position + 5 * Vector3.up, Vector3.down, out var hit, 100, GroundLayer);
        transform.position = hit.point + Vector3.up * particleYOffset;
        initial_position = transform.position;
        elapsed_time = 0;
        if(particle) particle.Play();
    }
    public override void OnUpdate()
    {
        elapsed_time += Time.deltaTime;
        Vector3 position = Vector3.Lerp(initial_position, initial_position + frontOffset * transform.forward, elapsed_time/lifetime);
        //Kode tidak akan jalan kalo stop harusnya;
        Collider[] hitTargets = Physics.OverlapBox(transform.position, Size, Quaternion.identity, AttackLayer);

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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + transform.forward * frontOffset, Size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * particleYOffset, 0.3f);
    }
#endif
}
