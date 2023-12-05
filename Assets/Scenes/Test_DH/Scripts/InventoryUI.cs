using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Image 클래스를 사용하기 위해 추가
using static UnityEditor.Progress;


public class InventoryUI : MonoBehaviour
{
    public static bool inventoryActivated = false; // 인벤토리가 열려있는동안 활동 금지
    [SerializeField]
    private GameObject go_InventoryBase; // 인벤토리의 베이스가 되는 이미지
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private HungryBar hungryBar;

    private slot[] slots; // 인벤토리 슬롯들

    public slot[] getSlots() { return slots; }

    [SerializeField] private Item[] items;

    public GameObject Items;

    [SerializeField] private StarterAssets.ThirdPersonController thePlayer;


    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum) 
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == _itemName)
            {
                slots[_arrayNum].AddItem(items[i], _itemNum);
            }
        }
    }

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<slot>(); // 인벤토리 슬롯들을 가져옴
    }


    public int AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType) // 슬롯이 비어있지 않다면
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName && slots[i].itemCount < 5) // 슬롯의 아이템 이름이 획득한 아이템 이름과 같다면
                    {
                        slots[i].SetSlotCount(_count); // 슬롯에 아이템 개수 추가
                        return 0;
                    }
                    else if (slots[i].item.itemName == _item.itemName && slots[i].itemCount == 5)
                    {
                        return 1;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return 0;
            }
        }
        return 2;
    }

    public void UsedItem(Item _item, int i)
    {
        if (slots[i].item != null && Item.ItemType.Used == _item.itemType)
        {
            hungryBar.Pb.BarValue += _item.itemValue;
            slots[i].SetSlotCount(-1);
            if (slots[i].itemCount <= 0)
                slots[i].ClearSlot();
            return;
        }
        else if(slots[i].item != null && Item.ItemType.Ingredient == _item.itemType)
        {

        }
    }

    public void ThrowItem(Item _item, int i, string name)
    {
        if (slots[i].item != null)
        {
            slots[i].SetSlotCount(-1);
            if (slots[i].itemCount <= 0)
                slots[i].ClearSlot();
            
            thePlayer.createprefabs(_item.itemPrefab, name);
            return;
        }
    }

    private bool SlotHasItem(string itemname, int itemcount) 
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == itemname && slots[i].itemCount == itemcount) // 슬롯의 아이템 이름이 획득한 아이템 이름과 같다면
                {
                    slots[i].SetSlotCount(-itemcount); // 슬롯에 아이템 감소
                    return true;
                }
            }
        }
        return false;
    }
}
