using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Combo
{

}
public class PlayerCombat : MonoBehaviour
{
    public List<Combo> combos = new();

    public float ComboInterval = 0.3f;

    private Coroutine coroutine;
    float time = 0;
    int index = 0;

    public void Attack() => coroutine ??= StartCoroutine(Attacking());
    public IEnumerator Attacking()
    {
        if (Time.time >= time || index + 1 >= combos.Count) index = 0;
        else index++;

        float ProjectileSpawnTime = combos[index].type == Attack_Type.projectile ? combos[index].FrameInWhichProjectileSpawn / combos[index].AnimationFrames : 10;
        string animationName = "Attack" + (index + 1).ToString();
        bool hasSpawned = false;

        LowerBody.Play(animationName);
        UpperBody.Play(animationName);
        yield return new WaitUntil(() => LowerBody.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        while (LowerBody.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            if (LowerBody.GetCurrentAnimatorStateInfo(0).normalizedTime >= ProjectileSpawnTime && !hasSpawned)
            {
                var obj = Instantiate(combos[index].Projectile, transform.position, transform.rotation);
                obj.GetComponent<Projectile>().StartBullet(transform.lossyScale.x);
                hasSpawned = true;
            }
            yield return null;
        }
        rb2d.mass -= MassIncreaseOnAttack;
        coroutine = null;
        time = Time.time + ComboInterval;
    }
}
