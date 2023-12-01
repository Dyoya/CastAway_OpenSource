using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public Sprite itemImage;  // ������ �̹���
    public string itemName;   // ������ �̸�
    public enum ItemType { Equipment, Used, Ingredient, ETC} // ������ Ÿ��
    public ItemType itemType; //�������� ����
    public GameObject itemPrefab;
    public int itemValue; //������ ȸ�� ��
    
    public string weaponType;
    
    public bool Food;
    public bool Perfect;
    public bool Cooked;
}
