using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    void Start()
    {
        transform.DOMoveZ(-10f, 7f);
        StartCoroutine(BirdDestroy());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane")) 
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BirdDestroy()
    {
        yield return new WaitForSeconds(7f); // 2√  ¥Î±‚
        Destroy(gameObject);
    }
}
