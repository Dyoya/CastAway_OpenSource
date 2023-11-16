using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TitleMenuEvent : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject OptionUI;

    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }

    // 게임 스타트 버튼 이벤트 함수입니다.
    public void StartButtonClicked()
    {
        //동현이가 잠시 추가해놓음 삭제때려도 됨
        SceneManager.LoadScene("Test_DH");
    }
    // 불러오기 버튼 이벤트 함수입니다.
    public void LoadButtonClicked()
    {

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
}
