using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public GameObject player;
    public float targetAlpha = 0.3f;
    public float changeAlphaTime = 0.5f;

    private GameObject lastHitObject;

    private void LateUpdate()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        int layerMask = 1 << LayerMask.NameToLayer("Environment");

        // 레이캐스트를 여러 번 사용해서 모든 오브젝트에 대한 정보를 개별적으로 얻어오기
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity, layerMask);

        // 거리에 따라 레이캐스트 히트 정보를 정렬
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        // 디버그
        Debug.DrawRay(transform.position, direction * Mathf.Infinity, Color.red);

        // 모든 히트된 오브젝트에 대해 처리
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name + " on layer: " + hit.collider.gameObject.layer);

            // 새로 맞은 오브젝트가 있을 때만 처리
            if (hit.collider.gameObject != lastHitObject)
            {
                // 현재 맞은 오브젝트가 이전과 다르다면 이전 오브젝트의 투명도를 서서히 원래대로 복원
                if (lastHitObject != null)
                {
                    StartCoroutine(FadeToOpaque(lastHitObject, 1f, changeAlphaTime));
                }

                // 새로 맞은 오브젝트의 투명도를 서서히 변경
                StartCoroutine(FadeToTransparent(hit.collider.gameObject, targetAlpha, changeAlphaTime));
                lastHitObject = hit.collider.gameObject;

                // 가장 가까운 오브젝트에 대한 처리만 진행
                break;
            }
        }

        // 레이가 어떤 오브젝트도 맞지 않았을 때 이전 오브젝트의 투명도를 서서히 복원
        if (hits.Length == 0 && lastHitObject != null)
        {
            StartCoroutine(FadeToOpaque(lastHitObject, 1f, changeAlphaTime));
            lastHitObject = null;
        }
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