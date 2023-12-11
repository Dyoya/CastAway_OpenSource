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
    public Item item; // ȹ���� ������
    public int itemCount; // ȹ���� ������ ����
    public Image itemImage; // �������� �̹���
    public Image Cooked; // �丮�� ������ ǥ��
    public Image Well;  // �� ������ �丮 ǥ��

    //�ʿ� ������Ʈ
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

        if(item.Cooked)         //�丮�� ������ �� Ȱ��ȭ
        {
            Cooked.gameObject.SetActive(true);
            if (item.Perfect)
                Well.gameObject.SetActive(true);
        }

        SetColor(1);
    }

    //������ �Һ�
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
        Cooked.gameObject.SetActive(false);
        Well.gameObject.SetActive(false);
    }
}
