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
    [SerializeField] private GameObject mapUI; // �����̰� �߰�����
    [SerializeField] private GameObject MinimapUI; // �����̰� �߰�����

    // UI�� ��� ���� �̺�Ʈ �Լ� �Դϴ�.
    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }

    // ���� ��ŸƮ ��ư �̺�Ʈ �Լ��Դϴ�.
    public void StartButtonClicked()
    {
        SceneManager.LoadScene(2);
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
    public void ResumeButtonClicked(GameObject obj)
    {
        GameManager.isPause = false;
        obj.SetActive(false);
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.isPause)
                CallMenu(PauseUI);
            else
                ResumeButtonClicked(PauseUI);
        }

        //�����̰� �߰��� �� UI ǥ��
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
        Time.timeScale = 0f; // �ð��� �帧 ����. 0���. �� �ð��� ����.
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
