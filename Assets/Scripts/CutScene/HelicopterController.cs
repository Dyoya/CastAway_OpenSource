using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    Vector3 currentLocalPosition;
    void Start()
    {
        StartCoroutine(HelicopterMove());
    }

    IEnumerator HelicopterMove()
    {
        yield return new WaitForSeconds(1f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition - new Vector3(-200, 0, 0), 20f);
        yield return new WaitForSeconds(15f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalRotate(new Vector3(0,-10, 0), 5f).SetEase(Ease.Linear);
        transform.DOLocalMove(currentLocalPosition - new Vector3(0, 0, -30), 5f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(5f);
        currentLocalPosition = transform.localPosition;
        transform.DOLocalMove(currentLocalPosition - new Vector3(0, 80, 0), 10f);
    }
}
