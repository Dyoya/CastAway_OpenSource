using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMarkerController : MonoBehaviour
{
    public RectTransform mapUI; // ���� UI ������Ʈ�� �����մϴ�.
    public Transform player; // �÷��̾� ������Ʈ�� �����մϴ�.
    public Image playerMarker; // �÷��̾� ��Ŀ �̹����� �����մϴ�.

    void Update()
    {
        // �÷��̾��� ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ�մϴ�.
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, player.position);

        // ��ũ�� ��ǥ�� ���� UI�� ���� ��ǥ�� ��ȯ�մϴ�.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapUI, screenPosition, null, out Vector2 localPosition);

        // �÷��̾� ��Ŀ�� ��ġ�� ������Ʈ�մϴ�.
        playerMarker.rectTransform.anchoredPosition = localPosition;
    }
}
