using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public Sprite itemImage;  // 아이콘 이미지
    public string itemName;   // 아이템 이름
    public enum ItemType { Equipment, Used, Ingredient, ETC} // 아이템 타입
    public ItemType itemType; //아이템의 유형
    public GameObject itemPrefab;
    public int itemValue; //아이템 회복 양
    
    public string weaponType;
    
    public bool Food;
    public bool Perfect;
    public bool Cooked;
}
