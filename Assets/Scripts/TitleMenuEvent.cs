using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System;
using StarterAssets;

public class TitleMenuEvent : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject OptionUI;
    private SaveAndLoad theSaveAndLoad;
    private TitleMenuEvent instance;
    private CharacterController thePlayer;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    // 게임 스타트 버튼 이벤트 함수입니다.
    public void StartButtonClicked()
    {
        SceneManager.LoadScene("PlaneCrashCutScene");
    }
    // 불러오기 버튼 이벤트 함수입니다.
    public void LoadButtonClicked()
    {
        Debug.Log("불러오기");
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Test_DW2");

        while (!operation.isDone)
        {
            yield return null;
        }

        thePlayer = FindObjectOfType<CharacterController>();
        thePlayer.GetComponent<ThirdPersonController>().enabled = false;
        theSaveAndLoad = FindAnyObjectByType<SaveAndLoad>();
        theSaveAndLoad.LoadData();
        yield return new WaitForSeconds(1f);
        thePlayer.GetComponent<ThirdPersonController>().enabled = true;
        gameObject.SetActive(false);
    }

    private void closeAllUI()
    {
        MainUI.SetActive(false);
        OptionUI.SetActive(false);
    }
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

    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}
