using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.TimeZoneInfo;

public class LightController : MonoBehaviour
{
    public Light sunLight;  // 햇빛을 조절할 라이트
    public Material[] skyboxes; // 여러 개의 Skybox를 저장할 배열
    public TextMeshProUGUI timeText;  // Text 오브젝트를 받아올 변수

    public Color dayColor = new Color(1.0f, 1.0f, 1.0f);      // 주간 색상 (흰색)
    public Color eveningColor = new Color(1.0f, 0.5f, 0.25f);  // 저녁 색상
    public Color nightColor = new Color(0.0f, 0.0f, 1.0f);     // 야간 색상 (파란색)

    public float dayDuration = 10.0f;       // 주간 지속 시간 (초)
    public float eveningDuration = 10.0f;   // 저녁 지속 시간 (초)
    public float nightDuration = 10.0f;     // 야간 지속 시간 (초)

    private float hourDuration;
    private int currentHour = 6;
    private float timeElapsed = 0.0f;  // 경과 시간

    public string formattedTime;

    void Start()
    {
        hourDuration = (dayDuration + eveningDuration + nightDuration) / 24;
        InvokeRepeating("UpdateTime", 0f, hourDuration); //함수호출 주기
    }
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (currentHour > 6 && currentHour < 16)
        {
            // Daytime (6 to 16)
            sunLight.color = dayColor;
        }
        else if (currentHour == 16 && timeElapsed < hourDuration)
        {
            // Transition from dayColor to eveningColor at the beginning of 16
            float t = Mathf.Clamp01(timeElapsed / hourDuration);
            sunLight.color = Color.Lerp(dayColor, eveningColor, t);
        }
        else if (currentHour > 16 && currentHour < 19)
        {
            // Evening (16 to 17)
            sunLight.color = eveningColor;
        }
        else if (currentHour == 19 && timeElapsed < hourDuration)
        {
            // Transition from eveningColor to nightColor at the beginning of 17
            float t = Mathf.Clamp01(timeElapsed / hourDuration);
            sunLight.color = Color.Lerp(eveningColor, nightColor, t);
        }
        else if ((currentHour > 19 && currentHour < 24) || (currentHour >= 0 && currentHour < 6))
        {
            // Night (17 to 24)
            sunLight.color = nightColor;
        }
        else if (currentHour == 6 && timeElapsed < hourDuration)
        {
            // Transition from nightColor to dayColor at the beginning of 24
            float t = Mathf.Clamp01(timeElapsed / hourDuration);
            sunLight.color = Color.Lerp(nightColor, dayColor, t);
        }
        else
        {
            // Reset timeElapsed when the full cycle is completed
            if (timeElapsed >= hourDuration)
            {
                timeElapsed = 0.0f;
            }
        }
    }
    void UpdateTime()
    {
        if (currentHour == 24)
        {
            currentHour = 0;
        }
        formattedTime = $"{currentHour++:D2}:00";
        timeText.text = formattedTime;
    }

    public void SetTime(string loadedFormattedTime)
    {
        formattedTime = loadedFormattedTime;
        currentHour = int.Parse(loadedFormattedTime.Substring(0, 2));
        timeText.text = formattedTime;

        float normalizedHour = currentHour / 24f;
        timeElapsed = hourDuration * normalizedHour;
    }
}