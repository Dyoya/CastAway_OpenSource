using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearStampTrigger : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int damaged = 10;

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� ������ �ֱ�
        Debug.Log("�÷��̾� �浹!");
    }
}
