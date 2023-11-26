using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //활성화 여부
    public static bool isActivate = false;

    //현재 장착된 Hand형 타입 무기
    [SerializeField]
    private CloseWeapon currentHand;

    //공격중
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
