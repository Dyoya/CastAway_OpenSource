using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject Pauseui;
    [SerializeField] private GameObject mapUI;
    [SerializeField] private GameObject MinimapUI;
    [SerializeField] private SaveAndLoad theSaveAndLoad;

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
        theSaveAndLoad.SaveData();
    }
    //pause 메뉴에서 메뉴로 돌아가기 버튼 이벤트 함수입니다.
    public void ExitButtonClicked()
    {
        Time.timeScale = 1f; // 1배속 (정상 속도)
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.isPause)
                CallMenu(Pauseui);
            else
                ResumeButtonClicked(Pauseui);
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
