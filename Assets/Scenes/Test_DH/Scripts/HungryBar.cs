using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryBar : MonoBehaviour
{
    public ProgressBar Pb;
    //������� 0�� ����� �ȉ����
    public bool isHungryZero = false;

    private void Start()
    {
        Pb.BarValue = 10;
    }

    //����� ����(�޸� ��)
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
    //����� ����
    public void DecreaseHungry()
    {
        if (Pb.BarValue <= 100)
        {
            Pb.BarValue -= Time.deltaTime * 0.5f;
        }
    }
}
