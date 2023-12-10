using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HelpManualController : MonoBehaviour
{
    public GameObject imageObject;

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (imageObject != null)
            {
                imageObject.SetActive(true);
            }
        }
        else
        {
            if (imageObject != null)
            {
                imageObject.SetActive(false);
            }
        }
    }
}