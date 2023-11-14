using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BearCutScene : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Bear());
    }
    IEnumerator Bear()
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("WalkF");
        Tweener walkTween =  transform.DOMove(new Vector3(120, 1.75f, -210), 5f).SetSpeedBased();
        yield return walkTween.WaitForCompletion();
        animator.SetTrigger("Buff");
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("WalkF");
        transform.DOMove(new Vector3(130, 1.75f, -190), 5f).SetSpeedBased();

        Vector3 direction = (new Vector3(130, 1.75f, -190) - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.DORotateQuaternion(lookRotation, 2.0f);
    }
}
