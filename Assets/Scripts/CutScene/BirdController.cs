using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float BirdMove;
    void Start()
    {
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
        yield return new WaitForSeconds(2.5f);
        transform.DOMove(new Vector3(-681, 310, -300), 20f);
        yield return new WaitForSeconds(5f); // 2√  ¥Î±‚
        Destroy(gameObject);
    }
}
