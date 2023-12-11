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
            text.text = "E : 불 피우기";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("모닥불 근처");
            
            // 불 피우기
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 나뭇가지 소모

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
