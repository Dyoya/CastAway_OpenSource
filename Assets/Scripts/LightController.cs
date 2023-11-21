using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class LightController : MonoBehaviour
{
    public Light sunLight;  // 햇빛을 조절할 라이트
    public Material[] skyboxes; // 여러 개의 Skybox를 저장할 배열

    public Color dayColor = new Color(1.0f, 1.0f, 1.0f);      // 주간 색상 (흰색)
    public Color eveningColor = new Color(1.0f, 0.5f, 0.25f);  // 저녁 색상
    public Color nightColor = new Color(0.0f, 0.0f, 1.0f);     // 야간 색상 (파란색)

    public float dayDuration = 5.0f;       // 주간 지속 시간 (초)
    public float eveningDuration = 5.0f;   // 저녁 지속 시간 (초)
    public float nightDuration = 1.0f;     // 야간 지속 시간 (초)

    private float timeElapsed = 0.0f;  // 경과 시간

    void Update()
    {
        // 시간에 따라 햇빛의 색상을 서서히 조절
        timeElapsed += Time.deltaTime;

        if (timeElapsed < dayDuration)
        {
            // 주간 시간
            float t = Mathf.Clamp01(timeElapsed / dayDuration);  // 0에서 1로 정규화
            sunLight.color = Color.Lerp(dayColor, eveningColor, t);
        }
        else if (timeElapsed < dayDuration + eveningDuration)
        {
            // 저녁 시간
            float t = (timeElapsed - dayDuration) / eveningDuration;
            sunLight.color = Color.Lerp(eveningColor, nightColor, t);
        }
        else
        {
            // 야간 시간
            float t = (timeElapsed - dayDuration - eveningDuration) / nightDuration;
            sunLight.color = Color.Lerp(nightColor, dayColor, t);

            // 전환 시간이 지나면 초기화
            if (timeElapsed >= dayDuration + eveningDuration + nightDuration)
            {
                timeElapsed = 0.0f;
            }
        }
    }
}