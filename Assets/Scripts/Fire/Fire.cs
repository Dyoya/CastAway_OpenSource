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
    public float currentDurationTime;

    [SerializeField]
    private ParticleSystem _ps;

    [SerializeField]
    private GameObject fire;

    [SerializeField] GameObject FireOffTrigger;
    [SerializeField] GameObject FireOnTrigger;

    bool isFire = false;

    GameObject player;
    ThirdPersonController _tpc;

    void Start()
    {
        player = GameObject.Find("Player");

        currentDurationTime = durationTime;

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
        currentDurationTime -= Time.deltaTime;

        if (currentDurationTime <= 0) 
        {
            FireOff();
        }

        if(_currentDamageTime > 0)
        {
            _currentDamageTime -= Time.deltaTime;
        }
    }
    public void addDurationTime(float time)
    {
        currentDurationTime += time;
    }

    private void FireOff()
    {
        _ps.Stop();
        isFire = false;
        FireOnTrigger.SetActive(false);
        FireOffTrigger.SetActive(true);

        StartCoroutine(SetFire(false));
    }

    public void FireOn(float time)
    {
        _ps.DORestart();
        isFire = true;
        FireOnTrigger.SetActive(true);
        FireOffTrigger.SetActive(false);

        currentDurationTime += time;

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
