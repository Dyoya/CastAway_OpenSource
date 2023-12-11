using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeCave : MonoBehaviour
{
    private SaveAndLoad theSaveAndLoad;
    private CharacterController thePlayer;
    private EscapeCave instance;
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
    IEnumerator LoadCoroutine()
    {
        theSaveAndLoad = FindAnyObjectByType<SaveAndLoad>();
        theSaveAndLoad.BossSaveData();
        AsyncOperation operation = SceneManager.LoadSceneAsync("Test_DH2");

        while (!operation.isDone)
        {
            yield return null;
        }

        thePlayer = FindObjectOfType<CharacterController>();
        thePlayer.GetComponent<ThirdPersonController>().enabled = false;
        theSaveAndLoad.LoadData();
        yield return new WaitForSeconds(0.1f);
        thePlayer.GetComponent<ThirdPersonController>().enabled = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            StartCoroutine(LoadCoroutine());
        }
    }
}
