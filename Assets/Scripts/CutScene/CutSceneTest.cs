using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CutSceneTest : MonoBehaviour
{
    public GameObject effectPrefab1; // 첫 번째 이펙트 프리팹
    public GameObject effectPrefab2;
    [SerializeField] Vector3 vec = new Vector3(0, 0, 0);
    [SerializeField] Vector3 vib = new Vector3 (0, 0, 0);
    [SerializeField] int vibrato = 0;
    private float timer = 0f;
    private float delay = 2f;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        transform.DOMove(new Vector3(-690, 307, -100), 6.5f).SetSpeedBased();

        transform.DOShakeRotation(6.5f, new Vector3(0, 0, 5), 10, 10, true);
        yield return new WaitForSeconds(6.5f); 

        if (effectPrefab1 != null)
        {
            // 첫 번째 이펙트 재생
            effectPrefab1.SetActive(true);
        }

        if (effectPrefab2 != null)
        {
            effectPrefab2.SetActive(true);
        }

        //transform.DORotate(new Vector3(10, 0, 0), 0.2f);
        //yield return new WaitForSeconds(0.2f);
        //transform.DOShakeRotation(10, vec, vibrato, 10, true);
        //yield return new WaitForSeconds(10f);
        //Destroy(gameObject);

        transform.DOMove(vec, 20.0f);
        Vector3 direction = (vec - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.DORotateQuaternion(lookRotation, 2.0f);
    }
}
