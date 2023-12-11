using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using static UnityEditor.Progress;

//, IPointerClickHandler

public class slot : MonoBehaviour
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템 개수
    public Image itemImage; // 아이템의 이미지
    public Image Cooked; // 요리된 아이템 표시
    public Image Well;  // 잘 구워진 요리 표시

    //필요 컴포넌트
    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private GameObject go_CountImage;
    [SerializeField]
    private InventoryUI inventoryUI;

    public int GetItemCount()
    {
        return itemCount;
    }
    public string GetItemName()
    {
        if (item != null)
            return item.itemName;
        return null;
    }

    public bool isFood()
    {
        return item.Food;
    }

    //이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    //아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        if(item.Cooked)         //요리된 음식일 시 활성화
        {
            Cooked.gameObject.SetActive(true);
            if (item.Perfect)
                Well.gameObject.SetActive(true);
        }

        SetColor(1);
    }

    //아이템 소비
    public void LeftHanduseItem(int hand)
    {
        inventoryUI.UsedItem(item, hand);
    }

    public void RightHanduseItem(int hand)
    {
        inventoryUI.UsedItem(item, hand);
    }

    public void LeftHandThrowItem(int hand, string name)
    {
        inventoryUI.ThrowItem(item, hand, name);
    }

    public void RightHandThrowItem(int hand, string name)
    {
        inventoryUI.ThrowItem(item, hand, name);
    }

    //아이템 슬롯 카운트 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    //슬롯 초기화
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
        Cooked.gameObject.SetActive(false);
        Well.gameObject.SetActive(false);
    }
}
