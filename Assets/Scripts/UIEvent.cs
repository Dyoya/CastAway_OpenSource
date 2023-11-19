using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UIEvent : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject OptionUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject mapUI; // 동현이가 추가했음
    [SerializeField] private GameObject MinimapUI; // 동현이가 추가했음

    // UI를 모두 끄는 이벤트 함수 입니다.
    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }

    // 게임 스타트 버튼 이벤트 함수입니다.
    public void StartButtonClicked()
    {
        SceneManager.LoadScene(2);
    }
    // 불러오기 버튼 이벤트 함수입니다.
    public void LoadButtonClicked()
    {
        Debug.Log("불러오기");
    }
    // 게임 메인화면으로 가는 버튼 이벤트 함수입니다.
    public void returnToMainButtonClicked()
    {
        closeAllUI();
        MainUI.SetActive(true);
    }
    // 옵션 버튼 이벤트 함수입니다.
    public void OptionButtonClicked()
    {
        closeAllUI();
        OptionUI.SetActive(true);
    }
    // 게임 종료 버튼 이벤트 함수입니다.
    public void QuitButtonClicked()
    {
        Application.Quit();
    }

    // pause 메뉴에서 계속하기 버튼 이벤트 함수입니다.
    public void ResumeButtonClicked(GameObject obj)
    {
        GameManager.isPause = false;
        obj.SetActive(false);
        Time.timeScale = 1f; // 1배속 (정상 속도)
    }

    // pause 메뉴에서 저장하기 버튼 이벤트 함수입니다.
    public void SaveButtonClicked()
    {
        Debug.Log("저장하기");
    }
    //pause 메뉴에서 메뉴로 돌아가기 버튼 이벤트 함수입니다.
    public void ExitButtonClicked()
    {
        SceneManager.LoadScene(0);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.isPause)
                CallMenu(PauseUI);
            else
                ResumeButtonClicked(PauseUI);
        }

        //동현이가 추가함 맵 UI 표시
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!GameManager.isPause)
            {
                MiniMapClose(MinimapUI);
                CallMenu(mapUI);
            }
            else
            {
                ResumeButtonClicked(mapUI);
                MiniMapOpen(MinimapUI);
            }

        }
    }
    private void CallMenu(GameObject obj)
    {
        GameManager.isPause = true;
        obj.SetActive(true);
        Time.timeScale = 0f; // 시간의 흐름 설정. 0배속. 즉 시간을 멈춤.
    }

    public void MiniMapClose(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void MiniMapOpen(GameObject obj)
    {
        obj.SetActive(true);
    }
}
