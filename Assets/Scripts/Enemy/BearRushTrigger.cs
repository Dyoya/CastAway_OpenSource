using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearRushTrigger : MonoBehaviour
{
    GameObject player;
    ThirdPersonController _tpc;

    [Header("Damage")]
    [SerializeField] int damage = 10;

    private void Start()
    {
        player = GameObject.Find("Player");
        _tpc = player.GetComponent<ThirdPersonController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 돌진 데미지 주기
        _tpc.AttackDamage(damage);

        Debug.Log("플레이어 충돌!");
    }
}
