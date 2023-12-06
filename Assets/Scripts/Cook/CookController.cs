using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class CookController : MonoBehaviour
{
    [SerializeField] private GameObject CookUI;
    public GameObject Player;
    public GameObject boxNote = null;
    [SerializeField] Transform Center = null;
    [SerializeField] RectTransform[] timingRect = null;
    [SerializeField] GameObject note = null;
    Vector2[] timingBoxs = null;

    [SerializeField] private slot Foodslot;
    [SerializeField] private slot Leftslot;
    [SerializeField] private slot Rightslot;
    [SerializeField] Transform CookedFoodFactory = null;
    private Item item;
    private int slot;

    private string perfectFood = "�߿丮��";
    private string normalFood = "�丮��";
    private string charredFood = "��";

    private void Start()
    {
        timingBoxs = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width / 2, Center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }

    public void CheckTiming()
    {
        float notePosX = boxNote.transform.localPosition.x;

        for (int i = 0; i < timingRect.Length;i++)
        {
            if (timingBoxs[i].x <= notePosX && notePosX <= timingBoxs[i].y)
            {
                CloseCookUI();
                if (i == 0)
                {
                    CookFood(slot, perfectFood);
                }
                if (i == 1)
                {
                    CookFood(slot, normalFood);
                }
                return;
            }
        }

        Debug.Log("Miss");
        CloseCookUI();
        CookFood(slot, charredFood);
    }

    private void Update()
    {
        FoodFrame();
    }

    private void FoodFrame()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            addFood(Leftslot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            addFood(Rightslot);
        }
        
    }

    private void CookFood(int slot, string prefix)          
    {
        if (slot == 1)
        {
            CreateCookedFood(Leftslot, prefix);
        }
        if (slot == 0)
        {
            CreateCookedFood(Rightslot, prefix);
        }
    }
    private void addFood(slot handslot)             // ���� �����ӿ� ���� �̹��� �߰��Լ�
    {
        if (handslot.item != null)
        {
            if (handslot.isFood())
            {
                Foodslot.AddItem(handslot.item);
                if (handslot == Leftslot)
                    slot = 1;
                else
                    slot = 0;
            }
            else
            {
                Debug.Log("������ �ƴմϴ�.");
            }
        }
        else
        {
            Debug.Log("�κ��丮�� ����ֽ��ϴ�.");
        }
    }

    private void CreateCookedFood(slot handslot, string prefix)    // �丮�� ������ �÷��̾� �տ� �����ϴ� �Լ�
    {
        Transform cookedFoodTransform;
        if (prefix == charredFood)
        {
            cookedFoodTransform = CookedFoodFactory.Find(prefix);
        }
        else
        {
            cookedFoodTransform = CookedFoodFactory.Find(prefix + handslot.GetItemName());
        }

        for (int i = 0; i < handslot.GetItemCount(); i++)
        {
            Vector3 spawnPosition = Player.transform.position + Player.transform.forward * 2f + new Vector3(0, 2, 0);
            GameObject cookedFoodInstance = Instantiate(cookedFoodTransform.gameObject, spawnPosition, Quaternion.identity);
            cookedFoodInstance.SetActive(true);
        }

        handslot.ClearSlot();
    }
    public void CloseCookUI()
    {
        CookUI.SetActive(false);
        GameManager.isPause = false;
        Player.GetComponent<ThirdPersonController>().enabled = true;
        Foodslot.ClearSlot();
    }
}
