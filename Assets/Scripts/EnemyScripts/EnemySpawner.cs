using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int maxEnemyCount = 5;

    private readonly List<SpawnSlot> _slots = new List<SpawnSlot>();

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            _slots.Add(new SpawnSlot(spawnPoints[i]));
        }
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            TrySpawnToLimit();
        }
    }

    private void Update()
    {
        if (_slots.Count == 0) return;
        if (maxEnemyCount <= 0) return;

        int alive = CountAlive();
        if (alive >= maxEnemyCount) return;

        for (int i = 0; i < _slots.Count && alive < maxEnemyCount; i++)
        {
            SpawnSlot slot = _slots[i];
            if (slot.Instance != null) continue;

            if (slot.NextSpawnTime <= 0f)
            {
                slot.NextSpawnTime = Time.time + respawnDelay;
                continue;
            }

            if (Time.time >= slot.NextSpawnTime)
            {
                Spawn(slot);
                slot.NextSpawnTime = 0f;
                alive++;
            }
        }
    }

    private void Spawn(SpawnSlot slot)
    {
        if (enemyPrefab == null || slot.Point == null) return;

        GameObject obj = Instantiate(enemyPrefab, slot.Point.position, slot.Point.rotation);
        slot.Instance = obj;
    }

    private int CountAlive()
    {
        int count = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Instance != null) count++;
        }

        return count;
    }

    private void TrySpawnToLimit()
    {
        if (_slots.Count == 0 || maxEnemyCount <= 0) return;

        int alive = CountAlive();
        for (int i = 0; i < _slots.Count && alive < maxEnemyCount; i++)
        {
            if (_slots[i].Instance != null) continue;
            Spawn(_slots[i]);
            alive++;
        }
    }

    private class SpawnSlot
    {
        public Transform Point { get; }
        public GameObject Instance;
        public float NextSpawnTime;

        public SpawnSlot(Transform point)
        {
            Point = point;
        }

    }
}
