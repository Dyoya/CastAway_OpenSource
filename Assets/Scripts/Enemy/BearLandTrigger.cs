using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearLandTrigger : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int damaged = 15;

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� ������ �ֱ�
        Debug.Log("�÷��̾� �浹!");
    }
}
