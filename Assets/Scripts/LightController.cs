using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightController : MonoBehaviour
{
    public Light sunLight;  
    public Material[] skyboxes; 
    public TextMeshProUGUI timeText;  

    public Color dayColor = new Color(1.0f, 1.0f, 1.0f);
    public Color eveningColor = new Color(1.0f, 0.5f, 0.25f);
    public Color nightColor = new Color(0.0f, 0.0f, 1.0f);

    public float dayDuration = 10.0f;
    public float eveningDuration = 10.0f;
    public float nightDuration = 10.0f;

    private float hourDuration;
    private int currentHour = 6;
    private float timeElapsed = 0.0f;

    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        hourDuration = (dayDuration + eveningDuration + nightDuration) / 24;
        InvokeRepeating("UpdateTime", 0f, hourDuration);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
  
        if (timeElapsed < dayDuration)
        {
            float t = Mathf.Clamp01(timeElapsed / dayDuration);
            sunLight.color = Color.Lerp(dayColor, eveningColor, t);
        }
        else if (timeElapsed < dayDuration + eveningDuration)
        {
            float t = (timeElapsed - dayDuration) / eveningDuration;
            sunLight.color = Color.Lerp(eveningColor, nightColor, t);
        }
        else
        {
            float t = (timeElapsed - dayDuration - eveningDuration) / nightDuration;
            sunLight.color = Color.Lerp(nightColor, dayColor, t);

            if (timeElapsed >= dayDuration + eveningDuration + nightDuration)
            {
                timeElapsed = 0.0f;
            }
        }
    }

    void UpdateTime()
    {
        if(currentHour == 24)
        {
            currentHour = 0;
        }
        string formattedTime = $"{currentHour++:D2}:00";
        timeText.text = formattedTime;
    }
}
