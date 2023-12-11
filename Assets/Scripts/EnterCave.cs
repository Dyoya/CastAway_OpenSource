using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterCave : MonoBehaviour
{
    private SaveAndLoad theSaveAndLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            other.gameObject.transform.position = new Vector3(-977, 33, 882);
            StartCoroutine(Enter());
        }
    }

    IEnumerator Enter()
    {       
        theSaveAndLoad = FindAnyObjectByType<SaveAndLoad>();
        theSaveAndLoad.SaveData();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadSceneAsync("BossCutScene");
    }
}
