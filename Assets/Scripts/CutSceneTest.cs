using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutSceneTest : MonoBehaviour
{
    public GameObject effectPrefab1; // 첫 번째 이펙트 프리팹
    public GameObject effectPrefab2;
    [SerializeField] Vector3 vec = new Vector3(0, 0, 0);
    [SerializeField] int vibrato = 0;
    private float timer = 0f;
    private float delay = 2f;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        transform.DOShakeRotation(2f, new Vector3(0, 0, 5), 10, 10, true);
        yield return new WaitForSeconds(2f); // 2초 대기

        if (effectPrefab1 != null)
        {
            // 첫 번째 이펙트 재생
            effectPrefab1.SetActive(true);
        }

        if (effectPrefab2 != null)
        {
            effectPrefab2.SetActive(true);
        }

        transform.DOShakeRotation(0.2f, vec, vibrato, 10, true);
        yield return new WaitForSeconds(0.2f);
        transform.DORotate(new Vector3(10, 0, 0), 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.DOShakeRotation(10, vec, vibrato, 10, true);
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
