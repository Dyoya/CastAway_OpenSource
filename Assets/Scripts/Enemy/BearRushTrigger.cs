using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearRushTrigger : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int damaged = 10;

    void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 돌진 데미지 주기
        Debug.Log("플레이어 충돌!");
    }
}
