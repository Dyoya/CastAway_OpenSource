using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    // 게임 세계의 100초가 현실 세계의 1초와 같게 만드는 변수
    [SerializeField]
    private float secondPerRealTimeSecond;

    private bool isNight = false;   // 밤인지 여부

    [SerializeField]
    private float fogDensityCalc; // 증감량 비율 계산

    [SerializeField]
    private float nightFogDensity;  // 밤 상태의 Fog 밀도
    private float dayFogDensity;    // 낮 상태의 Fog 밀도
    private float currentFogDensity;    // 계산에 사용할 현재 Fog 밀도

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
