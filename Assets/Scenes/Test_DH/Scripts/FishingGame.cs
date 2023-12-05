using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingGame : MonoBehaviour
{
    [SerializeField]
    private RectTransform playerBar; // 플레이어의 UI
    [SerializeField]
    private RectTransform fishBar; // 물고기의 UI
    [SerializeField]
    private Image gauge; // 게이지

    public Image catchFishImage; // Inspector에서 CatchFish 이미지를 연결해주세요.
    public GameObject fishPrefab; // Inspector에서 물고기 프리팹을 연결해주세요.
    public StarterAssets.ThirdPersonController player; // Inspector에서 Player 스크립트를 가진 오브젝트를 연결해주세요.

    private float gaugeSpeed = 0.001f;
    private float fishSpeed = 5f;
    private float playerSpeed = 1f;
    private float direction = 1f;

    private void Start()
    {
        StartCoroutine(ChangeDirection());
    }

    void Update()
    {
        PlayerControll();
        FishControll();
        GaugeControll();
    }

    private void PlayerControll()
    {
        if (Input.GetKey(KeyCode.UpArrow) && playerBar.anchoredPosition.y < 36.6f)
        {
            Debug.Log("up키적용");
            playerBar.anchoredPosition += new Vector2(0, playerSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow) && playerBar.anchoredPosition.y > -36.6f)
        {
            Debug.Log("down키적용");
            playerBar.anchoredPosition -= new Vector2(0, playerSpeed);
        }
    }

    private void FishControll()
    {
        float newY = fishBar.anchoredPosition.y + fishSpeed * direction;
        newY = Mathf.Clamp(newY, -43f, 43f);  // 물고기의 위치가 -36.6f와 36.6f 사이에 있도록 제한합니다.
        fishBar.anchoredPosition = new Vector2(0, newY);
        if (fishBar.anchoredPosition.y >= 43f || fishBar.anchoredPosition.y <= -43f)
            fishSpeed = -direction;
    }

    private void GaugeControll()
    {
        Debug.Log("Fish Y: " + fishBar.anchoredPosition.y);
        Debug.Log("Player Bar Top: " + (playerBar.anchoredPosition.y + playerBar.sizeDelta.y / 2));
        Debug.Log("Player Bar Bottom: " + (playerBar.anchoredPosition.y - playerBar.sizeDelta.y / 2));

        float fishSize = fishBar.sizeDelta.y * fishBar.localScale.y;
        float playerSize = playerBar.sizeDelta.y * playerBar.localScale.y;

        if (fishBar.anchoredPosition.y + fishSize / 2 >= playerBar.anchoredPosition.y - playerSize / 2 &&
            fishBar.anchoredPosition.y - fishSize / 2 <= playerBar.anchoredPosition.y + playerSize / 2)
        {
            gauge.fillAmount += gaugeSpeed;
            if (gauge.fillAmount >= 1.0f)
            {
                // CatchFish 이미지 띄우기
                catchFishImage.gameObject.SetActive(true);

                // 물고기 프리팹 생성
                player.createprefabs(fishPrefab, "물고기");
                gauge.fillAmount = 0.0f;
            }
        }
        else // 그렇지 않으면 게이지를 줄입니다.
        {
            gauge.fillAmount -= gaugeSpeed;
            if (gauge.fillAmount <= 0.0f)
            {
                Debug.Log("물고기를 놓쳤습니다.");

                catchFishImage.gameObject.SetActive(false);
                gauge.fillAmount = 0.0f;
            }
        }

        gauge.fillAmount = Mathf.Clamp(gauge.fillAmount, 0, 1);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 3));
            direction = -direction;
        }
    }
}
