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
        Pb.BarValue = 10;
    }

    //배고픔 감소(달릴 때)
    public void isRunningDecreaseHungry()
    {
        if (Pb.BarValue != 0)
        {
            Pb.BarValue -= Time.deltaTime * 1.5f;
        }
        else if (Pb.BarValue <= 0)
        {
            isHungryZero = true;
        }
    }
    //배고픔 감소
    public void DecreaseHungry()
    {
        if (Pb.BarValue <= 100)
        {
            Pb.BarValue -= Time.deltaTime * 0.5f;
        }
    }
}
