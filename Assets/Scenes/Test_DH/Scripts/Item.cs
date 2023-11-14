using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public Sprite itemImage;  // ������ �̹���
    public string itemName;   // ������ �̸�
    public enum ItemType { Equipment, Used, Ingredient, ETC, Apple, Steak, Tomato, Mushroom, Fish, Banana, None } // ������ Ÿ��
    public ItemType itemType; //�������� ����
    public GameObject itemPrefab;
    public int itemValue; //������ ȸ�� ��

    public string weaponType;
}
