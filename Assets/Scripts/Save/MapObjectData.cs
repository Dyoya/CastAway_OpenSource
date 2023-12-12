using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MapObjectData : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>(); // �ʿ� �ִ� �������� ������Ʈ
    [SerializeField] private List<GameObject> items = new List<GameObject>();   // �ݴ� ������Ʈ(��, �������� ��)

    public List<GameObject> GetObjects()
    {
        return objects;
    }

    public List<GameObject> GetItems()
    {
        return items;
    }

    public void DestroyMapItem(int i)
    {
        Destroy(items[i].gameObject);
    }

    public void DestroyObtainableObject(int i, bool destroy)
    {
        if (destroy == true)
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