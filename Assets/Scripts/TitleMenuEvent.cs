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
        SceneManager.LoadScene(2);
    }
    // 불러오기 버튼 이벤트 함수입니다.
    public void LoadButtonClicked()
    {
        Debug.Log("불러오기");
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
        {
            yield return null;
        }

        thePlayer = FindObjectOfType<CharacterController>();
        thePlayer.GetComponent<ThirdPersonController>().enabled = false;
        theSaveAndLoad = FindAnyObjectByType<SaveAndLoad>();
        theSaveAndLoad.LoadData();
        yield return new WaitForSeconds(1f);        // ThirdPersonController 스크립트가 켜져있으면 저장 위치로 이동이 안됨
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
