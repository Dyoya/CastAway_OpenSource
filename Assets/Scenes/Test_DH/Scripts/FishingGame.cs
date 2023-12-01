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

    private float gaugeSpeed = 0.0001f;
    private float fishSpeed = 2f;
    private float playerSpeed = 0.1f;

    void Update()
    {
        PlayerControll();
        FishControll();
        GaugeControll();
    }

    private void PlayerControll()
    {
        if (Input.GetKey(KeyCode.O) && playerBar.anchoredPosition.y < 36.6f)
        {
            Debug.Log("up키적용");
            playerBar.anchoredPosition += new Vector2(0, playerSpeed);
        }
        if (Input.GetKey(KeyCode.L) && playerBar.anchoredPosition.y > -36.6f)
        {
            Debug.Log("down키적용");
            playerBar.anchoredPosition -= new Vector2(0, playerSpeed);
        }
    }

    private void FishControll()
    {
        fishBar.anchoredPosition += new Vector2(0, fishSpeed);
        if (fishBar.anchoredPosition.y >= 36.6f || fishBar.anchoredPosition.y <= -36.6f)
            fishSpeed = -fishSpeed;
    }

    private void GaugeControll()
    {
        if (fishBar.anchoredPosition.y >= playerBar.anchoredPosition.y - playerBar.sizeDelta.y / 2 &&
            fishBar.anchoredPosition.y <= playerBar.anchoredPosition.y + playerBar.sizeDelta.y / 2)
            gauge.fillAmount += gaugeSpeed;
        else // 그렇지 않으면 게이지를 줄입니다.
            gauge.fillAmount -= gaugeSpeed;
    }
}
