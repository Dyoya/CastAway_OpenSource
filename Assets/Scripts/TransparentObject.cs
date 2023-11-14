using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public GameObject player;
    public float targetAlpha = 0.3f;
    public float changeAlphaTime = 1f;

    private GameObject lastHitObject;

    // 레이케스트를 통한 플레이어와 카메라 사이 모든 오브젝트에 대한 정보 배열
    private RaycastHit[] hits;
    private RaycastHit[] lastHits;

    private void Start()
    {
        UpdateLastHits();
    }
    // 마지막으로 히트딘 레이캐스트 히트 배열 갱신
    private void UpdateLastHits()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = (transform.position - playerPosition).normalized;
        int layerMask = 1 << LayerMask.NameToLayer("Environment");

        lastHits = Physics.RaycastAll(playerPosition, direction, Mathf.Infinity, layerMask);
    }

    private void LateUpdate()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = (transform.position - playerPosition).normalized;
        int layerMask = 1 << LayerMask.NameToLayer("Environment");

        hits = Physics.RaycastAll(playerPosition, direction, Mathf.Infinity, layerMask);

        if (!AreArraysEqual(hits, lastHits))
        {
            // 히트된 오브젝트에 대해 투명화 처리
            foreach (RaycastHit hit in hits)
            {
                StartCoroutine(FadeToTransparent(hit.collider.gameObject, targetAlpha, changeAlphaTime));
            }

            // 이전에 히트되었던 오브젝트에 대해 투명화 처리
            foreach (RaycastHit hit in lastHits)
            {
                // 이미 새로운 히트에 포함된 오브젝트는 스킵
                if (System.Array.Exists(hits, h => h.collider == hit.collider))
                    continue;

                StartCoroutine(FadeToOpaque(hit.collider.gameObject, 1f, changeAlphaTime));
            }

            // 현재 히트 정보를 이전 히트 정보로 갱신
            UpdateLastHits();
        }
    }
    // 두 배열의 요소가 모두 같은지 확인하는 함수
    private bool AreArraysEqual<T>(T[] arr1, T[] arr2)
    {
        if (arr1.Length != arr2.Length)
            return false;

        for (int i = 0; i < arr1.Length; i++)
        {
            if (!arr1[i].Equals(arr2[i]))
                return false;
        }

        return true;
    }
    private IEnumerator FadeToTransparent(GameObject obj, float targetAlpha, float duration)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.material.color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                renderer.material.color = renderer.material.color.WithAlpha(newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderer.material.color = renderer.material.color.WithAlpha(targetAlpha);
        }
    }
    private IEnumerator FadeToOpaque(GameObject obj, float targetAlpha, float duration)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.material.color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                renderer.material.color = renderer.material.color.WithAlpha(newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderer.material.color = renderer.material.color.WithAlpha(targetAlpha);
        }
    }
}