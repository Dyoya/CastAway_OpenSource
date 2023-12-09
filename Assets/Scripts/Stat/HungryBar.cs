using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryBar : MonoBehaviour
{
    public ProgressBar Pb;
    //배고픔이 0이 됬는지 안됬는지
    public bool isHungryZero = false;

    private void Start()
    {
        Pb.BarValue = 100;
    }

    public void SetBar(float value)
    {
        Pb.BarValue = value;
    }

    //배고픔 감소(달릴 때)
    public void DecreaseHungry(float Decreasespeed)
    {
        if (Pb.BarValue != 0)
        {
            Pb.BarValue -= Time.deltaTime * Decreasespeed;
            isHungryZero = false;
        }
        else if (Pb.BarValue == 0)
        {
            isHungryZero = true;
        }
    }
}
