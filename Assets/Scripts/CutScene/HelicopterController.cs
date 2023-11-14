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
        transform.DOMove(new Vector3(-763, 15, 18), 5);
        yield return new WaitForSeconds(5f);
        transform.DOMove(new Vector3(-763, -3.5f, 18), 5);
        yield return new WaitForSeconds(5f);
    }

    public void EscapeHelicopter()
    {
        StartCoroutine(Escape());
    }

    IEnumerator Escape()
    {
        transform.DOMove(new Vector3(-765.69f, 50, 16), 6);
        Debug.Log("Test1");
        yield return new WaitForSeconds(6f);
        transform.DOMove(new Vector3(-800f, 50, 16), 6);
        Debug.Log("Test2");
        transform.DORotate(new Vector3(0, 90, 0), 20f).SetEase(Ease.Linear);
        Debug.Log("Test3");
        transform.DOMove(new Vector3(-1000, 50, 300), 20f).SetEase(Ease.InOutQuad);
        Debug.Log("Test4");
    }
}
