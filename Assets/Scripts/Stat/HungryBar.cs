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
        Pb.BarValue = 100;
    }

    public void SetBar(float value)
    {
        Pb.BarValue = value;
    }

    //����� ����(�޸� ��)
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
