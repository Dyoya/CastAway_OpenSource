using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//, IPointerClickHandler

public class slot : MonoBehaviour
{
    public Item item; // ȹ���� ������
    public int itemCount; // ȹ���� ������ ����
    public Image itemImage; // �������� �̹���

    //�ʿ� ������Ʈ
    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private GameObject go_CountImage;
    [SerializeField]
    private InventoryUI inventoryUI;

    //�̹����� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    //������ ȹ��
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

    //������ �Һ�
    public void LeftHanduseItem()
    {
        inventoryUI.UsedItem(item, 0);
    }

    public void RightHanduseItem()
    {
        inventoryUI.UsedItem(item, 1);
    }

    //������ ���� ī��Ʈ ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    //���� �ʱ�ȭ
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
    //        Debug.Log("�̺�Ʈ �߻�");
    //        if (item != null)
    //        {
    //            if(item.itemType == Item.ItemType.Used)
    //            {
    //                Debug.Log(item.itemName + "�Һ� ������ ���");
    //                SetSlotCount(-1);
    //            }
    //            else
    //            {
    //                //������ �ִٸ� �ۼ�
    //            }
    //        }
    //    }
    //}
}
