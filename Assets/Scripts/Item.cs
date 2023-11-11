using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itenName;
    public Sprite itemImage;
    public int maximumNumber;
}