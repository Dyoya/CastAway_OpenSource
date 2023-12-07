using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyManualController : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public GameObject imageObject; // 이미지 오브젝트를 연결할 변수
    public float delayTime = 10f; // 숨기기까지의 대기 시간

    void Start()
    {
        // Text 객체를 보이게 설정
        if (textObject != null)
        {
            textObject.enabled = true;
        }

        // 10초 후에 HideText 함수를 호출
        Invoke("HideText", delayTime);
    }

    void Update()
    {
        // F1 키가 눌렸을 때
        if (Input.GetKey(KeyCode.F1))
        {
            // 이미지 오브젝트를 보이게 설정
            if (imageObject != null)
            {
                imageObject.SetActive(true);
            }
        }
        else
        {
            // F1 키가 눌리지 않았을 때 이미지 오브젝트를 숨기게 설정
            if (imageObject != null)
            {
                imageObject.SetActive(false);
            }
        }
    }

    void HideText()
    {
        // Text 객체를 숨기게 설정
        if (textObject != null)
        {
            textObject.enabled = false;
        }
    }
}

