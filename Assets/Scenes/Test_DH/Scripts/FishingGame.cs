using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingGame : MonoBehaviour
{
    [SerializeField]
    private RectTransform playerBar; // �÷��̾��� UI
    [SerializeField]
    private RectTransform fishBar; // ������� UI
    [SerializeField]
    private Image gauge; // ������

    public Image catchFishImage; // Inspector���� CatchFish �̹����� �������ּ���.
    public GameObject fishPrefab; // Inspector���� ����� �������� �������ּ���.
    public StarterAssets.ThirdPersonController player; // Inspector���� Player ��ũ��Ʈ�� ���� ������Ʈ�� �������ּ���.

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
            Debug.Log("upŰ����");
            playerBar.anchoredPosition += new Vector2(0, playerSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow) && playerBar.anchoredPosition.y > -36.6f)
        {
            Debug.Log("downŰ����");
            playerBar.anchoredPosition -= new Vector2(0, playerSpeed);
        }
    }

    private void FishControll()
    {
        float newY = fishBar.anchoredPosition.y + fishSpeed * direction;
        newY = Mathf.Clamp(newY, -43f, 43f);  // ������� ��ġ�� -36.6f�� 36.6f ���̿� �ֵ��� �����մϴ�.
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
                // CatchFish �̹��� ����
                catchFishImage.gameObject.SetActive(true);

                // ����� ������ ����
                player.createprefabs(fishPrefab, "�����");
                gauge.fillAmount = 0.0f;
            }
        }
        else // �׷��� ������ �������� ���Դϴ�.
        {
            gauge.fillAmount -= gaugeSpeed;
            if (gauge.fillAmount <= 0.0f)
            {
                Debug.Log("����⸦ ���ƽ��ϴ�.");

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
