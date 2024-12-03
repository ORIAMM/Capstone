using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PoolManager : MonoBehaviour
{
    public enum PooledInfo
    {
        GameObject,
        Canvas,
        Particle,
        None
    }
    private static Dictionary<string, List<GameObject>> ObjectPools = new();
    [SerializeField] private List<Transform> PoolParents = new();
    private static List<Transform> _PoolParents = new();
    private void Awake()
    {
        _PoolParents = PoolParents;
    }

    public static GameObject GetObject(GameObject ToSpawn, bool SetActiveToTrue = true, PooledInfo pooledInfo = PooledInfo.None)
    {
        if (!ObjectPools.ContainsKey(ToSpawn.name)) ObjectPools.Add(ToSpawn.name, new());
        List<GameObject> inactiveObjects = ObjectPools[ToSpawn.name];
        GameObject inactiveObject = inactiveObjects.FirstOrDefault();
        if (!inactiveObject)
        {
            inactiveObject = Instantiate(ToSpawn);
            inactiveObject.transform.SetParent(GetParent(pooledInfo));
        }
        else inactiveObjects.Remove(inactiveObject);
        inactiveObject.SetActive(SetActiveToTrue);
        return inactiveObject;
    }
    private static Transform GetParent(PooledInfo pooledInfo) => _PoolParents[(int)pooledInfo];
    public static void ReleaseObject(GameObject ToRelease)
    {
        string name = ToRelease.name[..^7]; // ^ (dari belakang) search geek4geek range indices
        ToRelease.SetActive(false);
        ObjectPools[name].Add(ToRelease);
    }
}