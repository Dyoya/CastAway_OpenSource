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
                text.text = "C : 요리 / E : 불씨 키우기(나뭇가지 1개)" + "[left : " + Mathf.FloorToInt(_fireScripts.currentDurationTime).ToString() + "s]";
                text.fontSize = 12;
            }
        }

        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("모닥불 근처");

            //불씨 키우기
            if (Input.GetKeyDown(KeyCode.E) && !keyPressed)
            {
                keyPressed = true;

                // 나뭇가지 1개 소모
                inventory.ConsumeItem("나뭇가지", 1);
                Debug.Log("불씨 증가!");

                _fireScripts.addDurationTime(addTime);

                StartCoroutine(Delay(0.3f));
            }
            // 요리
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
