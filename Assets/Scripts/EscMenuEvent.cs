using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuEvent : MonoBehaviour
{
    [SerializeField] private GameObject PauseUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.isPause)
                CallMenu();
            else
                ResumeButtonClicked();
        }
    }
    private void CallMenu()
    {
        GameManager.isPause = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f; // 시간의 흐름 설정. 0배속. 즉 시간을 멈춤.
    }

    public void ResumeButtonClicked()
    {
        GameManager.isPause = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f; // 1배속 (정상 속도)
    }

    public void SaveButtonClicked()
    {
        Debug.Log("세이브");
    }
    public void ExitButtonClicked()
    {
        Debug.Log("게임 종료");
        Application.Quit(); 
    }
}
