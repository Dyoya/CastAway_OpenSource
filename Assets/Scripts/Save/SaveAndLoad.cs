using JetBrains.Annotations;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    // 플레이어 위치 저장
    public Vector3 playerPos;
    public Vector3 playerRot;

    // 플레이어 상태 저장
    public float HealthBarValue;
    public float EnergyBarValue;
    public float HungryBarValue;

    // 인벤토리 저장 리스트
    public string Equipment;
    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>();

    // 맵 아이템 배치 저장 리스트
    public List<string> mapObjectName = new List<string>();
    public List<Vector3> mapObjectPositions = new List<Vector3>();
    public List<Vector3> mapObjectRotations = new List<Vector3>();

    public List<bool> DestroyedObject = new List<bool>();

    public int currentRockNum;
    public int currentFireNum;
    public string currentTime;

    public List<float> currentFire = new List<float>();
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
    private EndingTrigger theEndingTrigger;
    private LightController theLightController;
    private Fire theFire;

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
        theEndingTrigger = FindObjectOfType<EndingTrigger>();
        theLightController = FindObjectOfType<LightController>();
        theFire = FindObjectOfType<Fire>();

        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        saveData.HealthBarValue = theHealthBar.Pb.BarValue;
        saveData.EnergyBarValue = theEnergyBar.Pb.BarValue;
        saveData.HungryBarValue = theHungryBar.Pb.BarValue;
        saveData.currentRockNum = theEndingTrigger._currentRockNum;
        saveData.currentFireNum = theEndingTrigger._currentFireNum;
        saveData.currentTime = theLightController.formattedTime;

        // 저장전 데이터 초기화
        saveData.invenArrayNumber.Clear();
        saveData.invenItemName.Clear();
        saveData.invenItemNumber.Clear();
        saveData.Equipment = null;
        saveData.mapObjectName.Clear();
        saveData.mapObjectPositions.Clear();
        saveData.mapObjectRotations.Clear();
        saveData.DestroyedObject.Clear();
        saveData.currentFire.Clear();

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
            else
            {
                saveData.DestroyedObject.Add(true);
            }
        }

        List<GameObject> itemObjects = theMapObject.GetItems();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (itemObjects[i] != null)
            {
                saveData.mapObjectName.Add(itemObjects[i].name);
                saveData.mapObjectPositions.Add(itemObjects[i].transform.position);
                saveData.mapObjectRotations.Add(itemObjects[i].transform.eulerAngles);
                if (itemObjects[i].name == "FireWood")
                    saveData.currentFire.Add(itemObjects[i].transform.Find("Fire").Find("FireDamageTrigger").GetComponent<Fire>().currentDurationTime);
                else
                    saveData.currentFire.Add(0);
            }
            else
            {
                saveData.mapObjectName.Add(null);
                saveData.mapObjectPositions.Add(new Vector3(0, 0, 0));
                saveData.mapObjectRotations.Add(new Vector3(0, 0, 0));
                saveData.currentFire.Add(0);
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);
    }

    public void BossSaveData()
    {
        theInven = FindObjectOfType<InventoryUI>();
        theHealthBar = FindObjectOfType<HealthBar>();
        theEnergyBar = FindObjectOfType<EnergyBar>();
        theHungryBar = FindObjectOfType<HungryBar>();       

        saveData.HealthBarValue = theHealthBar.Pb.BarValue;
        saveData.EnergyBarValue = theEnergyBar.Pb.BarValue;
        saveData.HungryBarValue = theHungryBar.Pb.BarValue;

        saveData.invenArrayNumber.Clear();
        saveData.invenItemName.Clear();
        saveData.invenItemNumber.Clear();

        Debug.Log("보스 세이브");

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

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void BossLoadData()
    {
        string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
        saveData = JsonUtility.FromJson<SaveData>(loadJson);

        thePlayer = FindObjectOfType<CharacterController>();
        theInven = FindObjectOfType<InventoryUI>();
        theHealthBar = FindObjectOfType<HealthBar>();
        theEnergyBar = FindObjectOfType<EnergyBar>();
        theHungryBar = FindObjectOfType<HungryBar>();

        theHealthBar.SetBar(saveData.HealthBarValue);
        theEnergyBar.SetBar(saveData.EnergyBarValue);
        theHungryBar.SetBar(saveData.HungryBarValue);

        for (int i = 0; i < saveData.invenItemName.Count; i++)
        {
            theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            Debug.Log(saveData.invenItemName[i]);
        }

        if (saveData.Equipment != null)
        {
            ThirdPersonController TPC = thePlayer.GetComponent<ThirdPersonController>();
            switch (saveData.Equipment)
            {
                case "도끼":
                    TPC.hasAxe = true;
                    break;
                case "곡괭이":
                    TPC.hasPickAxe = true;
                    break;
                default:
                    break;
            }
        }
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
            theEndingTrigger = FindObjectOfType<EndingTrigger>();
            theLightController = FindObjectOfType<LightController>();

            theHealthBar.SetBar(saveData.HealthBarValue);
            theEnergyBar.SetBar(saveData.EnergyBarValue);
            theHungryBar.SetBar(saveData.HungryBarValue);

            theLightController.SetTime(saveData.currentTime);

            theEndingTrigger._currentRockNum = saveData.currentRockNum;
            theEndingTrigger._currentFireNum = saveData.currentFireNum;

            //플레이어 위치 로드
            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;

            GameObject loadedObject = null;

            int index = 0;

            yield return new WaitForSeconds(0.1f);

            Debug.Log("프리팹 리스트 개수 " + prefabList.Count);
            for (int i = 0; i < prefabList.Count; i++)
            {
                Debug.Log("프리팹 리스트 " + i + prefabList[i].name);
            }

            while (index < saveData.mapObjectName.Count)
            {
                if (index < initNum)
                {
                    if (saveData.mapObjectName == null || saveData.mapObjectName[index] == "")
                    {
                        theMapObject.DestroyMapItem(index);
                        index++;
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    if (saveData.mapObjectName != null || saveData.mapObjectName[index] != "")
                    {
                        while (loadedObject == null)
                        {
                            loadedObject = prefabList.Find(prefab => prefab.name == saveData.mapObjectName[index]);
                            yield return new WaitForSeconds(0.2f);
                        }

                        Debug.Log("saveObject Name " + saveData.mapObjectName[index]);
                        Debug.Log("loadedObject Name " + loadedObject);
                        GameObject newMapObject = Instantiate(loadedObject, saveData.mapObjectPositions[index], Quaternion.Euler(saveData.mapObjectRotations[index]));
                        newMapObject.name = loadedObject.name;
                        theMapObject.AddItemObjects(newMapObject);
                        Debug.Log("불 시간" + saveData.currentFire[index]);
                        if (newMapObject.name == "FireWood")
                        {
                            newMapObject.transform.Find("Fire").Find("FireDamageTrigger").GetComponent<Fire>().addDurationTime(saveData.currentFire[index]);
                            newMapObject.transform.Find("Fire").Find("FireDamageTrigger").GetComponent<Fire>().FireOn();
                        }
                        loadedObject = null;
                        index++;
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            // 1번 2번 인벤토리 로드
            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            // 파괴된 맵 오브젝트 (나무, 돌 등) 삭제
            for (int i = 0; i < saveData.DestroyedObject.Count; i++)
            {
                theMapObject.DestroyObtainableObject(i, saveData.DestroyedObject[i]);
            }

            // 플레이어가 들고있는 장비 로드
            if (saveData.Equipment != null)
            {
                ThirdPersonController TPC = thePlayer.GetComponent<ThirdPersonController>();
                switch (saveData.Equipment)
                {
                    case "도끼":
                        TPC.hasAxe = true;
                        break;
                    case "곡괭이":
                        TPC.hasPickAxe = true;
                        break;
                    default:
                        break;
                }
            }        
        }
        yield return null;
    }
}
