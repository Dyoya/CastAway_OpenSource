using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HelicopterMove());
    }

    IEnumerator HelicopterMove()
    {
        transform.DOMove(new Vector3(10, 15, 9), 5);
        yield return new WaitForSeconds(5f);
        transform.DOMove(new Vector3(10, -0.5f, 9), 5);
        yield return new WaitForSeconds(5f);
    }
}
