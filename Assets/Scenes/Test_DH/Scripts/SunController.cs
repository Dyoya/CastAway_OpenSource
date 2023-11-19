using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    // ���� ������ 100�ʰ� ���� ������ 1�ʿ� ���� ����� ����
    [SerializeField]
    private float secondPerRealTimeSecond;

    private bool isNight = false;   // ������ ����

    [SerializeField]
    private float fogDensityCalc; // ������ ���� ���

    [SerializeField]
    private float nightFogDensity;  // �� ������ Fog �е�
    private float dayFogDensity;    // �� ������ Fog �е�
    private float currentFogDensity;    // ��꿡 ����� ���� Fog �е�

    // Start is called before the first frame update
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        if(transform.eulerAngles.x >= 170)
        {
            if (transform.eulerAngles.x >= 340)
                isNight = false;
            else
                isNight = true;
        }   

        if (isNight)
        {
            if(currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }

    }
}
