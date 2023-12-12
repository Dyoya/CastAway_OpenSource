using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireTextTrigger : MonoBehaviour
{
    GameObject _player;
    [SerializeField] GameObject textUI;
    [SerializeField] TextMeshProUGUI text;
    GameObject FireGameUI;
    FireGame _fg;
    [SerializeField] InventoryUI inventory;

    void Start()
    {
        _player = GameObject.Find("Player");
        FireGameUI = GameObject.Find("UI").transform.Find("FireGameUI").gameObject;
        _fg = FireGameUI.GetComponent<FireGame>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(true);
            text.text = "E : �� �ǿ�� (�������� 1��)";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("��ں� ��ó");
            
            // �� �ǿ��
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (inventory.SlotHasItem("��������", 1))
                {
                    // �������� 1�� �Ҹ�
                    inventory.ConsumeItem("��������", 1);
                    _fg.FireWood = transform.parent.gameObject.transform.Find("FireDamageTrigger").gameObject;

                    textUI.SetActive(false);
                    GameManager.isPause = true;
                    _player.GetComponent<ThirdPersonController>().enabled = false;
                    _fg.StartGame();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(false);
            _fg.StopGame();
        }
    }
}
