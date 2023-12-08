using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Bear")]
    [SerializeField] GameObject BearPrefab;
    [SerializeField] int MaxBearNum = 5;
    [SerializeField] GameObject[] BearSpawnPoint;
    int _bearSpawnPointNum;
    int _currentBearNum = 0;

    [Header("Rabbit")]
    [SerializeField] GameObject RabbitPrefab;
    [SerializeField] int MaxRabbitNum = 10;
    [SerializeField] GameObject[] RabitSpawnPoint;
    int _rabbitSpawnPointNum;
    int _currentRabbitNum = 0;

    void Start()
    {
        _bearSpawnPointNum = BearSpawnPoint.Length;
        _rabbitSpawnPointNum = RabitSpawnPoint.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnBear()
    {
        
    }
}
