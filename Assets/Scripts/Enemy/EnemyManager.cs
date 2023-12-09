using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Bear")]
    [SerializeField] GameObject BearPrefab;
    [SerializeField] int MaxBearNum = 5;
    int _currentBearNum = 0;
    [SerializeField] GameObject[] BearSpawnPoint;
    int _bearSpawnPointNum;
    [SerializeField] float BearSpawnTime = 10f;
    

    [Header("Rabbit")]
    [SerializeField] GameObject RabbitPrefab;
    [SerializeField] int MaxRabbitNum = 10;
    int _currentRabbitNum = 0;
    [SerializeField] GameObject[] RabbitSpawnPoint;
    int _rabbitSpawnPointNum;
    [SerializeField] float RabbitSpawnTime = 10f;


    void Start()
    {
        _bearSpawnPointNum = BearSpawnPoint.Length;
        _rabbitSpawnPointNum = RabbitSpawnPoint.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentBearNum < MaxBearNum)
        {
            spawnBear();
        }

        if(_currentRabbitNum < MaxRabbitNum)
        {
            spawnRabbit();
        }
    }

    // °õ ½ºÆù ÇÔ¼ö
    void spawnBear()
    {
        _currentBearNum++;
        int num = Random.Range(0, _bearSpawnPointNum);
        GameObject bear = Instantiate(BearPrefab, BearSpawnPoint[num].transform);
    }
    // °õ »ç¸ÁÃ³¸® Áö¿¬ ÇÔ¼ö
    IEnumerator BearDieCoroutine()
    {
        yield return new WaitForSeconds(BearSpawnTime);
        _currentBearNum--;
    }
    // °õ »ç¸ÁÃ³¸® ÆÛºí¸¯ ÇÔ¼ö
    public void BearDie()
    {
        StartCoroutine(BearDieCoroutine());
    }

    // Åä³¢ ½ºÆù ÇÔ¼ö
    void spawnRabbit()
    {
        _currentRabbitNum++;
        int num = Random.Range(0, _rabbitSpawnPointNum);
        GameObject rabbit = Instantiate(RabbitPrefab, RabbitSpawnPoint[num].transform);
    }
    // Åä³¢ »ç¸ÁÃ³¸® Áö¿¬ ÇÔ¼ö
    IEnumerator RabbitDieCoroutine()
    {
        yield return new WaitForSeconds(RabbitSpawnTime);
        _currentRabbitNum--;
    }
    // Åä³¢ »ç¸ÁÃ³¸® ÆÛºí¸¯ ÇÔ¼ö
    public void RabbitDie()
    {
        StartCoroutine(RabbitDieCoroutine());
    }
}
