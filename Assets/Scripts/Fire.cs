using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField] private float damageTime;
    float _currentDamageTime;

    [SerializeField] private float durationTime;
    float _currentDurationTime;

    [SerializeField]
    private ParticleSystem _ps;

    [SerializeField]
    private GameObject fire;

    bool isFire = true;

    GameObject player;
    ThirdPersonController _tpc;

    void Start()
    {
        player = GameObject.Find("Player");

        _currentDurationTime = durationTime;

        _tpc = player.GetComponent<ThirdPersonController>();
    }


    void Update()
    {
        if(isFire)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        _currentDurationTime -= Time.deltaTime;

        if (_currentDurationTime <= 0) 
        {
            FireOff();
        }

        if(_currentDamageTime > 0)
        {
            _currentDamageTime -= Time.deltaTime;
        }
    }

    private void FireOff()
    {
        _ps.Stop();
        isFire = false;

        StartCoroutine(SetFire(false));
    }

    public void FireOn(float time)
    {
        _ps.DORestart();
        isFire = true;
        _currentDurationTime += time;

        StartCoroutine(SetFire(true));
    }

    IEnumerator SetFire(bool onoff)
    {
        yield return new WaitForSeconds(3f);

        fire.SetActive(onoff);
    }

    private void OnTriggerStay(Collider other)
    {
        if(isFire && other.transform.tag == "Player")
        {
            if(_currentDamageTime <= 0)
            {
                Debug.Log("불 데미지");
                // 플레이어 데미지 주기 코드 작성
                _tpc.AttackDamage(damage);

                _currentDamageTime = damageTime;
            }
        }
    }
}
