using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.TimeZoneInfo;

public class LightController : MonoBehaviour
{
    public Light sunLight;  // �޺��� ������ ����Ʈ
    public Material[] skyboxes; // ���� ���� Skybox�� ������ �迭
    public TextMeshProUGUI timeText;  // Text ������Ʈ�� �޾ƿ� ����

    public Color dayColor = new Color(1.0f, 1.0f, 1.0f);      // �ְ� ���� (���)
    public Color eveningColor = new Color(1.0f, 0.5f, 0.25f);  // ���� ����
    public Color nightColor = new Color(0.0f, 0.0f, 1.0f);     // �߰� ���� (�Ķ���)

    public float dayDuration = 10.0f;       // �ְ� ���� �ð� (��)
    public float eveningDuration = 10.0f;   // ���� ���� �ð� (��)
    public float nightDuration = 10.0f;     // �߰� ���� �ð� (��)

    private float hourDuration;
    private int currentHour = 6;
    private float timeElapsed = 0.0f;  // ��� �ð�

    public string formattedTime;

    void Start()
    {
        hourDuration = (dayDuration + eveningDuration + nightDuration) / 24;
        InvokeRepeating("UpdateTime", 0f, hourDuration); //�Լ�ȣ�� �ֱ�
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