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
        Time.timeScale = 0f; // �ð��� �帧 ����. 0���. �� �ð��� ����.
    }

    public void ResumeButtonClicked()
    {
        GameManager.isPause = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f; // 1��� (���� �ӵ�)
    }

    public void SaveButtonClicked()
    {
        Debug.Log("���̺�");
    }
    public void ExitButtonClicked()
    {
        Debug.Log("���� ����");
        Application.Quit(); 
    }
}
