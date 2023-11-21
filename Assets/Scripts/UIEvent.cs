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

    public static UIEvent instance;
    
    private SaveAndLoad theSaveAndLoad;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //    Destroy(this.gameObject);
    }

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

        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Test_DH");

        while(!operation.isDone)
        {
            yield return null;
        }

        theSaveAndLoad = FindObjectOfType<SaveAndLoad>();
        theSaveAndLoad.LoadData();
        Destroy(gameObject);
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
}
