using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuEvent : MonoBehaviour
{
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject mapUI; // 동현이가 추가했음

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
                CallMenu(mapUI);
            else
                ResumeButtonClicked(mapUI);
        }
    }
    private void CallMenu(GameObject obj)
    {
        GameManager.isPause = true;
        obj.SetActive(true);
        Time.timeScale = 0f; // 시간의 흐름 설정. 0배속. 즉 시간을 멈춤.
    }

    public void ResumeButtonClicked(GameObject obj)
    {
        GameManager.isPause = false;
        obj.SetActive(false);
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
