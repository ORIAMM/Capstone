using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage;
    public float duration;
    public float range;
    public LayerMask targetLayer;

    public void Initialize(BossBehaviour boss)
    {
        damage = boss.ATK;
        duration = boss.SkillDuration;
        range = boss.AtkRange;
        Destroy(gameObject, duration);

        StartCoroutine(DealDamageWithSphere());
    }

    private System.Collections.IEnumerator DealDamageWithSphere()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            Collider[] hitTargets = Physics.OverlapSphere(transform.position, range, targetLayer);

            foreach (Collider target in hitTargets)
            {
                IEntity entity = target.GetComponent<IEntity>();
                if (entity != null)
                {
                    entity.ReceiveDamage(damage);
                }
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
