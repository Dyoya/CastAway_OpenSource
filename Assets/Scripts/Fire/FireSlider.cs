using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireSlider : MonoBehaviour
{
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (slider.value <= 0)
        {
            transform.Find("Fill Area").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Fill Area").gameObject.SetActive(true);
        }
    }
}
