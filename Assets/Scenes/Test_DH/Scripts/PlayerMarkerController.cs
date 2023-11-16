using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMarkerController : MonoBehaviour
{
    public RectTransform mapUI; // 지도 UI 오브젝트를 연결합니다.
    public Transform player; // 플레이어 오브젝트를 연결합니다.
    public Image playerMarker; // 플레이어 마커 이미지를 연결합니다.

    void Update()
    {
        // 플레이어의 월드 좌표를 스크린 좌표로 변환합니다.
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, player.position);

        // 스크린 좌표를 지도 UI의 로컬 좌표로 변환합니다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapUI, screenPosition, null, out Vector2 localPosition);

        // 플레이어 마커의 위치를 업데이트합니다.
        playerMarker.rectTransform.anchoredPosition = localPosition;
    }
}
