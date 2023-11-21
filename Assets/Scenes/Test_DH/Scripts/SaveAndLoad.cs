using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public List<int> inventoryArrayNum = new List<int>();
    public List<string> inventoryItemName = new List<string>();
    public List<int> inventoryItemNum = new List<int>();
}

public class SaveAndLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private StarterAssets.ThirdPersonController thePlayer;
    private InventoryUI theInventory;


    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if(!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }

    public void SaveData()
    {
        thePlayer = FindObjectOfType<StarterAssets.ThirdPersonController>();
        theInventory = FindObjectOfType<InventoryUI>();

        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;


        slot[] slots = theInventory.getSlots();
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                saveData.inventoryArrayNum.Add(i);
                saveData.inventoryItemName.Add(slots[i].item.itemName);
                saveData.inventoryItemNum.Add(slots[i].itemCount);
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장됨");
        Debug.Log(json);
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);
            Debug.Log(loadJson);
            thePlayer = FindAnyObjectByType<StarterAssets.ThirdPersonController>();
            theInventory = FindAnyObjectByType<InventoryUI>();

            if(thePlayer == null)
            {
                Debug.Log("thePlayer is null");
            }
            else
            {
                thePlayer.transform.position = saveData.playerPos;
                thePlayer.transform.eulerAngles = saveData.playerRot;

                for(int i = 0; i < saveData.inventoryItemName.Count; i++)
                {
                    theInventory.LoadToInven(saveData.inventoryArrayNum[i], saveData.inventoryItemName[i], saveData.inventoryItemNum[i]);
                }

                Debug.Log("로드 완료");
            }
        }
        else
        {
            Debug.Log("세이브 파일이 없습니다.");
        }
    }
}
