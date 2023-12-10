using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class BearCutScene : MonoBehaviour
{
    private Animator animator;
    private SaveAndLoad theSaveAndLoad;
    private CharacterController thePlayer;
    private BearCutScene instance;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Bear());
    }
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
    IEnumerator Bear()
    {
        animator.SetBool("Sleep", true);
        yield return new WaitForSeconds(23f);
        animator.SetBool("Sleep", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("WakeUp", true);
        yield return new WaitForSeconds(3f);
        animator.SetBool("ShoutOut", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("ShoutOut", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Angry", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Angry", false);
        yield return new WaitForSeconds(2f);
        StartCoroutine(LoadCoroutine());
        yield return null;
    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Boss");

        while (!operation.isDone)
        {
            yield return null;
        }

        thePlayer = FindObjectOfType<CharacterController>();
        thePlayer.GetComponent<ThirdPersonController>().enabled = false;
        theSaveAndLoad = FindAnyObjectByType<SaveAndLoad>();
        theSaveAndLoad.BossLoadData();
        yield return new WaitForSeconds(0.1f);
        thePlayer.GetComponent<ThirdPersonController>().enabled = true;
        gameObject.SetActive(false);
    }
}
