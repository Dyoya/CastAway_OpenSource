using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField] GameObject textUI;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] GameObject[] rockPrefab;
    int _totalRockNum;
    public int _currentRockNum = 0;

    [SerializeField] GameObject[] firePrefab;
    int _totalFireNum;
    public int _currentFireNum = 0;

    int activeFire = 0;

    float _endingDelayTime = 5f;
    float _currentTime = 0;

    void Start()
    {
        _totalRockNum = rockPrefab.Length;
        _totalFireNum = firePrefab.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // 엔딩 조건 확인
        foreach (GameObject fire in firePrefab)
        {
            if (fire.transform.Find("FireDamageTrigger").GetComponent<Fire>().currentDurationTime > 0)
            {
                activeFire++;
            }
        }
        Debug.Log(activeFire);
        if(activeFire == _totalFireNum)
        {
            _currentTime += Time.deltaTime;
            if(_currentTime > _endingDelayTime)
            {
                // 엔딩, 게임 종료

                Debug.Log("엔딩");
            }
        }

        activeFire = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && (_currentRockNum < _totalRockNum))
        {
            textUI.SetActive(true);

            text.text = "E : 구조 요청 준비하기 (돌 1개)";
        }
        else if (other.gameObject.tag == "Player" && (_currentFireNum < _totalFireNum))
        {
            textUI.SetActive(true);

            text.text = "E : 구조 요청 준비하기 (나뭇가지 5개)";
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_currentRockNum < _totalRockNum)
                {
                    // 돌 소모

                    rockPrefab[_currentRockNum].SetActive(true);
                    _currentRockNum++;
                    //Debug.Log(_currentRockNum);
                }
                else
                {
                    if (_currentFireNum < _totalFireNum)
                    {
                        // 나무 소모

                        firePrefab[_currentFireNum].SetActive(true);
                        _currentFireNum++;
                        //Debug.Log(_currentFireNum);
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(false);
        }
    }
}
