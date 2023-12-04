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
        animator.SetBool("Sleep", true);
        yield return new WaitForSeconds(23f);
        animator.SetBool("Sleep", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("WakeUp", true);
        yield return new WaitForSeconds(3f);
        animator.SetBool("ShoutOut", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("ShoutOut", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Angry", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Angry", false);

    }
}
