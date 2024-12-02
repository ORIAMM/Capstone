using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage;
    public float duration;

    public void Initialize(BossBehaviour boss)
    {
        damage = boss.ATK;
        duration = boss.SkillDuration;
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            IEntity target = other.GetComponent<IEntity>();
            if (target != null)
            {
                target.ReceiveDamage(damage);
            }
        }
    }
}
