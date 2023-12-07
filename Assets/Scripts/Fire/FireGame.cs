using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.VisualScripting;

public class FireGame : MonoBehaviour
{
    GameObject _player;
    GameObject FireGameUI;
    
    [SerializeField] float goalTime = 10f;
    [SerializeField] GameObject FireTrigger;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI TimeText;

    float currentTime;
    bool isGame;

    private void Start()
    {
        _player = GameObject.Find("Player");
        currentTime = goalTime;
    }

    private void Update()
    {
        if(isGame)
        {
            currentTime -= Time.deltaTime;
            slider.value -= 0.2f * Time.deltaTime;

            // 게임 성공
            if (slider.value >= 0.95) 
            {
                StopGame();
                FireTrigger.GetComponent<Fire>().FireOn(60f);
            }

            // 게임 실패
            if(currentTime <= 0)
            {
                currentTime = 0;
                StopGame();
            }

            SetText(currentTime);
        }
        else
        {
            initSetting();
        }
    }
    void SetText(float time)
    {
        
        TimeText.text = "Time : " + Mathf.FloorToInt(time).ToString() + "s";
    }
   
    void initSetting()
    {
        currentTime = goalTime;
        slider.value = 0;
    }
    public void clickButton()
    {
        slider.value += 0.05f;
    }

    public void StartGame(GameObject FireGameUI)
    {
        this.FireGameUI = FireGameUI;
        isGame = true;
        this.FireGameUI.SetActive(true);
    }
    public void StopGame()
    {
        isGame = false;
        if(FireGameUI != null)
        {
            this.FireGameUI.SetActive(false);
        }
        GameManager.isPause = false;
        _player.GetComponent<ThirdPersonController>().enabled = true;
    }
}
