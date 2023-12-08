using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCutScenePlayer : MonoBehaviour
{
    Vector3 currentLocalPosition;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(HappyPlayer());
    }
    IEnumerator HappyPlayer()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Looking", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Looking", false);
        yield return new WaitForSeconds(4f);
        animator.SetBool("Waving", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Waving", false);
        yield return new WaitForSeconds(23f);
        animator.SetBool("Cheering", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Victory", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Victory", false);
        animator.SetBool("Walk", true);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition + new Vector3(-5, 0, 0), 5f).SetEase(Ease.InOutQuad);
    }
}
