using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //Ȱ��ȭ ����
    public static bool isActivate = false;

    //���� ������ Hand�� Ÿ�� ����
    [SerializeField]
    private CloseWeapon currentHand;

    //������
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if(!isAttack)
            {
                //StartCoroutine(Attackcoroutine());
            }
        }
    }

    //IEnumerator Attackcoroutine()
    //{
    //    isAttack = true;
    //    currentHand.anim.SetTrigger("Attack");


    //}

}
