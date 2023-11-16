using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    public ProgressBar Pb;

    private void Start()
    {
        Pb.BarValue = 5;
    }

    //스테미너 감소
    public void DecreaseStamina()
    {
        if (Pb.BarValue != 0)
        {
            Pb.BarValue -= Time.deltaTime * 8;
        }
    }
    //스테미너 증가
    public void RecoverStamina()
    {
        if (Pb.BarValue < 100)
        {
            Pb.BarValue += Time.deltaTime * 4;
        }
    }
}