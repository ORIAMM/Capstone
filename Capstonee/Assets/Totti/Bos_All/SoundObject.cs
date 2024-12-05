using UnityEngine;

public class SoundObject : MonoBehaviour
{
    [SerializeField] string SFX_Name;
    private void OnEnable()
    {
        SoundManager.instance.PlaySFX(SFX_Name);
        PoolManager.ReleaseObject(gameObject);
    }
}
