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
        theSaveAndLoad.SaveData();
    }
    //pause �޴����� �޴��� ���ư��� ��ư �̺�Ʈ �Լ��Դϴ�.
    public void ExitButtonClicked()
    {
        Time.timeScale = 1f; // 1��� (���� �ӵ�)
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
