using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // �÷��̾� ��ġ ����
    public Vector3 playerPos;
    public Vector3 playerRot;

    // �÷��̾� ���� ����
    public float HealthBarValue;
    public float EnergyBarValue;
    public float HungryBarValue;

    // �κ��丮 ���� ����Ʈ
    public string Equipment;
    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>();
    public List<bool> mapObject = new List<bool>();

    // �� ������ ��ġ ���� ����Ʈ
    public List<string> MapObjectName = new List<string>();
    public List<Vector3> mapObjectPositions = new List<Vector3>();
    public List<Vector3> mapObjectRotations = new List<Vector3>();

    public List<bool> DestroyedObject = new List<bool>();
}

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();
    public int initNum;

    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private CharacterController thePlayer;
    private InventoryUI theInven;
    private MapObjectData theMapObject;
    private HealthBar theHealthBar;
    private EnergyBar theEnergyBar;
    private HungryBar theHungryBar;

    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
        {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }

    public void SaveData()
    {
        thePlayer = FindObjectOfType<CharacterController>();
        theInven = FindObjectOfType<InventoryUI>();
        theMapObject = FindObjectOfType<MapObjectData>();
        theHealthBar = FindObjectOfType<HealthBar>();
        theEnergyBar = FindObjectOfType<EnergyBar>();
        theHungryBar = FindObjectOfType<HungryBar>();


        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        saveData.HealthBarValue = theHealthBar.Pb.BarValue;
        saveData.EnergyBarValue = theEnergyBar.Pb.BarValue;
        saveData.HungryBarValue = theHungryBar.Pb.BarValue;

        // ������ ������ �ʱ�ȭ
        saveData.invenArrayNumber.Clear();
        saveData.invenItemName.Clear();
        saveData.invenItemNumber.Clear();
        saveData.Equipment = null;
        saveData.MapObjectName.Clear();
        saveData.mapObjectPositions.Clear();
        saveData.mapObjectRotations.Clear();
        saveData.DestroyedObject.Clear();

        slot[] slots = theInven.getSlots();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
                if (slots[i].item.itemType == Item.ItemType.Equipment)
                    saveData.Equipment = slots[i].item.itemName;
            }
        }

        List<GameObject> objects = theMapObject.GetObjects();



        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
            {
                saveData.DestroyedObject.Add(objects[i].GetComponent<ObtainableObject>().getDestroyed());
            }
        }

        List<GameObject> itemObjects = theMapObject.GetItems();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i] != null)
            {
                saveData.MapObjectName.Add(itemObjects[i].name);
                saveData.mapObjectPositions.Add(itemObjects[i].transform.position);
                saveData.mapObjectRotations.Add(itemObjects[i].transform.eulerAngles);
            }
            else
            {
                saveData.MapObjectName.Add(null);
                saveData.mapObjectPositions.Add(new Vector3(0, 0, 0));
                saveData.mapObjectRotations.Add(new Vector3(0, 0, 0));
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("���� �Ϸ�");
        Debug.Log(json);
    }

    public void LoadData()
    {
        StartCoroutine(LoadDataCoroutine());
    }
    IEnumerator LoadDataCoroutine()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer = FindObjectOfType<CharacterController>();
            theInven = FindObjectOfType<InventoryUI>();
            theMapObject = FindObjectOfType<MapObjectData>();
            theHealthBar = FindObjectOfType<HealthBar>();
            theEnergyBar = FindObjectOfType<EnergyBar>();
            theHungryBar = FindObjectOfType<HungryBar>();

            theHealthBar.SetBar(saveData.HealthBarValue);
            theEnergyBar.SetBar(saveData.EnergyBarValue);
            theHungryBar.SetBar(saveData.HungryBarValue);

            //�÷��̾� ��ġ �ε�
            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;

            // 1�� 2�� �κ��丮 �ε�
            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            // �ı��� �� ������Ʈ (����, �� ��) ����
            for (int i = 0; i < saveData.DestroyedObject.Count; i++)
            {
                theMapObject.DestroyObtainableObject(i, saveData.DestroyedObject[i]);
            }

            // �÷��̾ ����ִ� ��� �ε�
            if (saveData.Equipment != null)
            {
                ThirdPersonController TPC = thePlayer.GetComponent<ThirdPersonController>();
                switch (saveData.Equipment)
                {
                    case "����":
                        TPC.hasAxe = true;
                        break;
                    case "���":
                        TPC.hasPickAxe = true;
                        break;
                    default:
                        break;
                }
            }
            GameObject loadedObject = null;
           
            while (initNum < saveData.MapObjectName.Count)
            {
                Debug.Log("initNum " + initNum + "saveData.MapObjectName[initNum] " + saveData.MapObjectName[initNum]);
                if (saveData.MapObjectName[initNum] != null && saveData.MapObjectName[initNum] != "")
                {
                    loadedObject = prefabList.Find(prefab => prefab.name == saveData.MapObjectName[initNum]);
                    if (loadedObject == null)
                    {
                        Debug.Log("������Ʈ�� ����");
                        loadedObject = prefabList.Find(prefab => prefab.name == saveData.MapObjectName[initNum]);
                    }
                    else
                    {
                        Debug.Log("������Ʈ ã�� " + loadedObject + " " + initNum + " ������Ʈ �̸�" + loadedObject.name);
                        GameObject newMapObject = Instantiate(loadedObject, saveData.mapObjectPositions[initNum], Quaternion.Euler(saveData.mapObjectRotations[initNum]));
                        newMapObject.name = loadedObject.name;
                        theMapObject.AddItemObjects(newMapObject);
                        loadedObject = null;
                        initNum++;
                    }
                }
                else
                {
                    Debug.Log("����");
                    initNum++;
                }              
            }
            yield return null;
        }
    }
}
