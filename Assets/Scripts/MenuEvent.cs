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

    // UI�� ��� ���� �̺�Ʈ �Լ� �Դϴ�.
    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }

    // ���� ��ŸƮ ��ư �̺�Ʈ �Լ��Դϴ�.
    public void StartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
    // �ҷ����� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void LoadButtonClicked()
    {
        Debug.Log("�ҷ�����");
    }
    // ���� ����ȭ������ ���� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void returnToMainButtonClicked()
    {
        closeAllUI();
        MainUI.SetActive(true);
    }
    // �ɼ� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void OptionButtonClicked()
    {
        closeAllUI();
        OptionUI.SetActive(true);
    }
    // ���� ���� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void QuitButtonClicked()
    {
        Application.Quit();
    }

    // pause �޴����� ����ϱ� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void ResumeButtonClicked()
    {
        GameManager.isPause = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f; // 1��� (���� �ӵ�)
    }

    // pause �޴����� �����ϱ� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void SaveButtonClicked()
    {
        Debug.Log("�����ϱ�");
    }
    //pause �޴����� �޴��� ���ư��� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void ExitButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    [SerializeField] private GameObject PauseUI;
    void Update()
    {
        // ���� �޴��� �ƴҶ� esc ��ư�� ������ ��� ����
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
        Time.timeScale = 0f; // �ð��� �帧 ����. 0���. �� �ð��� ����.
    }   
}
