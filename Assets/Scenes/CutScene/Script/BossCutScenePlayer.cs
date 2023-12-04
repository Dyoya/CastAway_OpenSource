using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCutScenePlayer : MonoBehaviour
{
    private Animator animator;
    Vector3 currentLocalPosition;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(EncounterBoss());
        //Tweener walkTween = transform.DOMove(new Vector3(-5f, 0, 0), 5f, true);
    }
    IEnumerator EncounterBoss()
    {
        currentLocalPosition = transform.localPosition;
        animator.SetBool("Walk", true); 
        transform.DOLocalMove(currentLocalPosition - new Vector3(10, 0, 0), 6f);
        yield return new WaitForSeconds(3f);
        animator.SetBool("Walk", false);
        animator.SetBool("LookAround", true);
        yield return new WaitForSeconds(3f);
        animator.SetBool("Scared", true);
        animator.SetBool("LookAround", false);
        yield return new WaitForSeconds(10f);
        animator.SetBool("BackWard", true);
        animator.SetBool("Scared", false);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition - new Vector3(-5, 0, 0), 5f);
        yield return new WaitForSeconds(3f);
        animator.SetBool("BackWard", false);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("FallDown", true);
    }
}
