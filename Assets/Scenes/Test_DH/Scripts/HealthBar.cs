using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public ProgressBar Pb;
    public HungryBar hungry;
    public bool Playernotsurvival = false;

    private void Start()
    {
        Pb.BarValue = 10;
    }

    //������� 0�� �Ǿ��� ��
    public void ZeroHungry()
    {
        if (hungry.isHungryZero)
        {
            if (Pb.BarValue != 0)
                Pb.BarValue -= Time.deltaTime * 1.5f;
        }
        else if (Pb.BarValue == 0)
        {
            Playernotsurvival = true;
        }
    }
}
