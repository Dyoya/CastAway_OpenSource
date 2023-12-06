using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireOnTrigger : MonoBehaviour
{
    GameObject _player;
    [SerializeField] GameObject Fire;
    [SerializeField] GameObject textUI;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float addTime = 10f;
    Fire _fireScripts;
    bool keyPressed = false;

    void Start()
    {
        _player = GameObject.Find("Player");
        _fireScripts = Fire.GetComponent<Fire>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(true);
            text.text = "E : �Ҿ� Ű���";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("��ں� ��ó");

            if (Input.GetKeyDown(KeyCode.E) && !keyPressed)
            {
                keyPressed = true;

                // �������� �Ҹ�
                Debug.Log("�Ҿ� ����!");

                _fireScripts.addDurationTime(addTime);

                StartCoroutine(Delay(0.5f));
            }
        }
    }
    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        keyPressed = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(false);
        }
    }
}
