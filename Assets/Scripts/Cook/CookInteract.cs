using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CookInteract : MonoBehaviour
{
    [SerializeField] private GameObject CookUI;
    public GameObject Player;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("¸ð´ÚºÒ À§");
            if (Input.GetKey(KeyCode.E))
            {
                GameManager.isPause = true;
                Player.GetComponent<ThirdPersonController>().enabled = false;
                CookUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CookUI.SetActive(false);
            GameManager.isPause = false;
            Player.GetComponent<ThirdPersonController>().enabled = true;
        }
    }
}
