using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KeyManualController : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public GameObject imageObject; 
    public float delayTime = 10f;
    void Start()
    {
        if (textObject != null)
        {
            textObject.enabled = true;
        }
        Invoke("HideText", delayTime);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            if (imageObject != null)
            {
                imageObject.SetActive(true);
            }
        }
        else
        {
            if (imageObject != null)
            {
                imageObject.SetActive(false);
            }
        }
    }
    void HideText()
    {
        if (textObject != null)
        {
            textObject.enabled = false;
        }
    }
}