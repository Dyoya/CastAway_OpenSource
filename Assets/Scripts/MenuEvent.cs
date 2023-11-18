using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MenuEvent : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject OptionUI;

    // UI를 모두 끄는 이벤트 함수 입니다.
    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }

    // 게임 스타트 버튼 이벤트 함수입니다.
    public void StartButtonClicked()
    {
        SceneManager.LoadScene(1);
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
    public void ResumeButtonClicked()
    {
        GameManager.isPause = false;
        PauseUI.SetActive(false);
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

    [SerializeField] private GameObject PauseUI;
    void Update()
    {
        // 메인 메뉴가 아닐때 esc 버튼을 눌렀을 경우 정지
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!GameManager.isPause)
                    CallMenu();
                else
                    ResumeButtonClicked();
            }
        }
    }
    private void CallMenu()
    {
        GameManager.isPause = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f; // 시간의 흐름 설정. 0배속. 즉 시간을 멈춤.
    }   
}
