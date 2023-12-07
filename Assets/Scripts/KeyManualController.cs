using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyManualController : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public GameObject imageObject; // �̹��� ������Ʈ�� ������ ����
    public float delayTime = 10f; // ���������� ��� �ð�

    void Start()
    {
        // Text ��ü�� ���̰� ����
        if (textObject != null)
        {
            textObject.enabled = true;
        }

        // 10�� �Ŀ� HideText �Լ��� ȣ��
        Invoke("HideText", delayTime);
    }

    void Update()
    {
        // F1 Ű�� ������ ��
        if (Input.GetKey(KeyCode.F1))
        {
            // �̹��� ������Ʈ�� ���̰� ����
            if (imageObject != null)
            {
                imageObject.SetActive(true);
            }
        }
        else
        {
            // F1 Ű�� ������ �ʾ��� �� �̹��� ������Ʈ�� ����� ����
            if (imageObject != null)
            {
                imageObject.SetActive(false);
            }
        }
    }

    void HideText()
    {
        // Text ��ü�� ����� ����
        if (textObject != null)
        {
            textObject.enabled = false;
        }
    }
}

