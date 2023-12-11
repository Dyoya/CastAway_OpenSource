using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireTextTrigger : MonoBehaviour
{
    GameObject _player;
    [SerializeField] GameObject FireGameUI;
    [SerializeField] GameObject textUI;
    [SerializeField] TextMeshProUGUI text;
    FireGame _fg;

    void Start()
    {
        _player = GameObject.Find("Player");
        _fg = GetComponent<FireGame>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(true);
            text.text = "E : �� �ǿ��";
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
                // �������� �Ҹ�

                textUI.SetActive(false);
                GameManager.isPause = true;
                _player.GetComponent<ThirdPersonController>().enabled = false;
                _fg.StartGame(FireGameUI);
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
