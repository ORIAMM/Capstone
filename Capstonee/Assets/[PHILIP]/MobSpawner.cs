using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject mobPrefab;
    public List<Transform> spawnLocations = new();
    public int spawnCount = 0;

    private void OnEnable()
    {
        List<int> spawns = new();
        for (int i = 0; i < spawnCount; i++)
        {
            int rand;
            do
            {
                rand = Random.Range(0, spawnLocations.Count);
            }while(spawns.Contains(rand));
            spawns.Add(rand);
            var obj = PoolManager.GetObject(mobPrefab, false);
            obj.transform.position = spawnLocations[rand].position;
            obj.SetActive(true);
        }
        PoolManager.ReleaseObject(gameObject);
    }
}
