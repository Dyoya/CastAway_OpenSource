using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//, IPointerClickHandler

public class slot : MonoBehaviour
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템 개수
    public Image itemImage; // 아이템의 이미지

    //필요 컴포넌트
    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private GameObject go_CountImage;
    [SerializeField]
    private InventoryUI inventoryUI;

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
        SetColor(1);
    }

    //아이템 소비
    public void LeftHanduseItem()
    {
        inventoryUI.UsedItem(item, 0);
    }

    public void RightHanduseItem()
    {
        inventoryUI.UsedItem(item, 1);
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
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if(eventData.button == PointerEventData.InputButton.Right)
    //    {
    //        Debug.Log("이벤트 발생");
    //        if (item != null)
    //        {
    //            if(item.itemType == Item.ItemType.Used)
    //            {
    //                Debug.Log(item.itemName + "소비 아이템 사용");
    //                SetSlotCount(-1);
    //            }
    //            else
    //            {
    //                //장착이 있다면 작성
    //            }
    //        }
    //    }
    //}
}
