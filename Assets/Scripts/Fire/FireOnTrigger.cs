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

    [SerializeField] GameObject CookUI;

    [SerializeField] InventoryUI inventory;

    bool isCooked = false;
    Fire _fireScripts;
    bool keyPressed = false;

    void Start()
    {
        _player = GameObject.Find("Player");
        _fireScripts = Fire.GetComponent<Fire>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(!isCooked)
            {
                textUI.SetActive(true);
                text.text = "C : �丮 / E : �Ҿ� Ű���(�������� 1��)" + "[left : " + Mathf.FloorToInt(_fireScripts.currentDurationTime).ToString() + "s]";
                text.fontSize = 12;
            }
        }

        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("��ں� ��ó");

            //�Ҿ� Ű���
            if (Input.GetKeyDown(KeyCode.E) && !keyPressed)
            {
                keyPressed = true;

                // �������� 1�� �Ҹ�
                inventory.ConsumeItem("��������", 1);
                Debug.Log("�Ҿ� ����!");

                _fireScripts.addDurationTime(addTime);

                StartCoroutine(Delay(0.3f));
            }
            // �丮
            if (Input.GetKey(KeyCode.C))
            {
                isCooked = true;
                textUI.SetActive(false);
                GameManager.isPause = true;
                _player.GetComponent<ThirdPersonController>().enabled = false;
                CookUI.SetActive(true);
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

            CookUI.SetActive(false);
            GameManager.isPause = false;
            _player.GetComponent<ThirdPersonController>().enabled = true;
            isCooked = false;

            text.fontSize = 20;
        }
    }
}
