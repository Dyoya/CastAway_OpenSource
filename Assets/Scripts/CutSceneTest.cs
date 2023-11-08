using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutSceneTest : MonoBehaviour
{
    // Update is called once per frame  
    void Start()
    {
        transform.DOShakeRotation(10, 10, 10, 10, true);
    }
}
