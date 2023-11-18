using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class LightController : MonoBehaviour
{
    public Light sunLight;  // �޺��� ������ ����Ʈ
    public Material[] skyboxes; // ���� ���� Skybox�� ������ �迭

    public Color dayColor = new Color(1.0f, 1.0f, 1.0f);      // �ְ� ���� (���)
    public Color eveningColor = new Color(1.0f, 0.5f, 0.25f);  // ���� ����
    public Color nightColor = new Color(0.0f, 0.0f, 1.0f);     // �߰� ���� (�Ķ���)

    public float dayDuration = 5.0f;       // �ְ� ���� �ð� (��)
    public float eveningDuration = 5.0f;   // ���� ���� �ð� (��)
    public float nightDuration = 1.0f;     // �߰� ���� �ð� (��)

    private float timeElapsed = 0.0f;  // ��� �ð�

    void Update()
    {
        // �ð��� ���� �޺��� ������ ������ ����
        timeElapsed += Time.deltaTime;

        if (timeElapsed < dayDuration)
        {
            // �ְ� �ð�
            float t = Mathf.Clamp01(timeElapsed / dayDuration);  // 0���� 1�� ����ȭ
            sunLight.color = Color.Lerp(dayColor, eveningColor, t);
            RenderSettings.skybox.Lerp(skyboxes[0], skyboxes[1], t);
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(skyboxes[0].GetFloat("_Exposure"), skyboxes[1].GetFloat("_Exposure"), t));
        }
        else if (timeElapsed < dayDuration + eveningDuration)
        {
            // ���� �ð�
            float t = (timeElapsed - dayDuration) / eveningDuration;
            sunLight.color = Color.Lerp(eveningColor, nightColor, t);
            RenderSettings.skybox.Lerp(skyboxes[1], skyboxes[2], t);
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(skyboxes[1].GetFloat("_Exposure"), skyboxes[2].GetFloat("_Exposure"), t));
        }
        else
        {
            // �߰� �ð�
            float t = (timeElapsed - dayDuration - eveningDuration) / nightDuration;
            sunLight.color = Color.Lerp(nightColor, dayColor, t);
            RenderSettings.skybox.Lerp(skyboxes[2], skyboxes[0], t);
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(skyboxes[2].GetFloat("_Exposure"), skyboxes[0].GetFloat("_Exposure"), t));

            // ��ȯ �ð��� ������ �ʱ�ȭ
            if (timeElapsed >= dayDuration + eveningDuration + nightDuration)
            {
                timeElapsed = 0.0f;
            }
        }
    }
}