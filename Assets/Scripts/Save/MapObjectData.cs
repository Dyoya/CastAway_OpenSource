using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Progress;

public class MapObjectData : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>(); // 맵에 있는 나무같은 오브젝트
    [SerializeField] private List<GameObject> items = new List<GameObject>();   // 줍는 오브젝트(돌, 나뭇가지 등)

    public List<GameObject> GetObjects()
    {
        return objects;
    }

    public List<GameObject> GetItems()
    {
        return items;
    }

    public void DestroyObtainableObject(int i, bool destroy)
    {
        if (destroy && i >= 0 && i < objects.Count)
        {
            objects[i].GetComponent<ObtainableObject>().DestroyMe();
        }
    }
    public void AddItemObjects(GameObject obj)
    {
        items.Add(obj);
    }
    public void RemoveItemObjects(GameObject obj)
    {
        if (obj != null)
        {
            items.Remove(obj);
        }
    }
}