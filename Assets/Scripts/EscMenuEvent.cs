using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuEvent : MonoBehaviour
{
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject mapUI; // �����̰� �߰�����
    [SerializeField] private GameObject MinimapUI; // �����̰� �߰�����

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

    public void ResumeButtonClicked(GameObject obj)
    {
        GameManager.isPause = false;
        obj.SetActive(false);
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

    public void MiniMapClose(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void MiniMapOpen(GameObject obj)
    {
        obj.SetActive(true);
    }
}
