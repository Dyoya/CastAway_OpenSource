using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Craft
{
    public string craftName; // �̸�
    public GameObject go_prefab; // ���� ��ġ �� ������
    public GameObject go_PreviewPrefab; // �̸� ���� ������
}

public class CraftManual : MonoBehaviour
{
    private bool isActivated = false;  // CraftManual UI Ȱ�� ����
    private bool isPreviewActivated = false; // �̸� ���� Ȱ��ȭ ����

    [SerializeField]
    private GameObject go_BaseUI; // �⺻ ���̽� UI

    [SerializeField]
    private Craft[] craft_fire; //�� �ǿ� �ִ� ���Ե�. 
    private GameObject go_Preview; // �̸� ���� �������� ���� ����
    private GameObject go_Prefab; // ���� ������ �������� ���� ���� 

    [SerializeField]
    private Transform tf_Player;  // �÷��̾� ��ġ

    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //�����̰� �߰���
    [SerializeField]
    private GameObject[] slots;

    [SerializeField]
    private InventoryUI inventory;

    [SerializeField]
    private StarterAssets.ThirdPersonController thePlayer;

    MapObjectData theMapObject;

    private List<string> setItemName = new List<string>();
    private List<int> setItemCount = new List<int>();

    public bool isbuilder = false;

    public void SlotClick(int _slotNumber)
    {
        Debug.Log(_slotNumber);
        GameObject slot = slots[_slotNumber];

        if (!slot.activeInHierarchy)
            return;

        TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>();

        TextMeshProUGUI ExplainText = null;

        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name == "ExplainText")
            {
                ExplainText = text;
                break;
            }
        }

        if (ExplainText != null)
        {
            string[] lines = ExplainText.text.Split('\n'); // �ؽ�Ʈ�� �ٹٲ� ���ڸ� �������� �и�

            Dictionary<string, int> materials = new Dictionary<string, int>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(' '); // �ؽ�Ʈ�� ������ �������� �и�

                string itemName = parts[0];
                int itemCount = int.Parse(parts[1]);

                setItemName.Add(itemName);
                setItemCount.Add(itemCount);

                // ��ųʸ��� ��� �߰�
                materials[itemName] = itemCount;
            }

            bool hasAllMaterials = true; // ��� ��ᰡ �־�ߵ�
            foreach (KeyValuePair<string, int> material in materials)
            {
                if (!inventory.SlotHasItem(material.Key, material.Value))
                {
                    hasAllMaterials = false;
                    break;
                }
            }
            Debug.Log(hasAllMaterials);
            if (hasAllMaterials && _slotNumber <= 2)
            {
                go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
                go_Prefab = craft_fire[_slotNumber].go_prefab;
                isPreviewActivated = true;
                go_BaseUI.SetActive(false);
                isActivated = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if(hasAllMaterials && _slotNumber >= 3)
            {
                for (int i = 0; i < setItemName.Count; i++)
                {
                    inventory.ConsumeItem(setItemName[i], setItemCount[i]);
                }
                GameObject itemPrefab = GetItemPrefabBasedOnSlotNumber(_slotNumber);
                string itemName = GetItemNameBasedOnSlotNumber(_slotNumber);
                thePlayer.createprefabs(itemPrefab, itemName);
            }
        }
    }

    [SerializeField]
    private GameObject[] prefab;
    private GameObject GetItemPrefabBasedOnSlotNumber(int number)
    {   
        return prefab[number];
    }

    private string GetItemNameBasedOnSlotNumber(int number)
    {
        return prefab[number].name;
    }


    void Start()
    {
        go_BaseUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isPreviewActivated)
        {
            isbuilder = true;
            Window();
            GameManager.isPause = !GameManager.isPause;
        }

        if (isPreviewActivated)
        {
            PreviewPositionUpdate();
            GameManager.isPause = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Build();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isbuilder = false;
            Cancel();
        }
    }

    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
                go_Preview.SetActive(true);

                //Debug.Log(_location);
                //Debug.Log(go_Preview.transform.position);
            }
            else
            {
                go_Preview.SetActive(false);
            }
        }
        else
            go_Preview.SetActive(false);
    }

    //��ư Ŭ���Ǹ� ����
    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            GameObject build = Instantiate(go_Prefab, hitInfo.point, Quaternion.identity);
            build.name = go_Prefab.name;
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
            theMapObject = FindObjectOfType<MapObjectData>();
            theMapObject.AddItemObjects(build);

            // ������ �Һ��� �Ǽ��ϵ��� ����
            for (int i = 0; i < setItemName.Count; i++)
            {
                inventory.ConsumeItem(setItemName[i], setItemCount[i]);
            }
        }
    }
    //TabŰ ������ ����
    private void Window()
    {   
        if (!isActivated) //â ���������� â�� Ŵ
            OpenWindow();
        else
            CloseWindow(); // â ���������� â�� ����
    }

    private void OpenWindow()
    {
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
    }
    //ESC������ ����
    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        go_Preview = null;
        go_Prefab = null;

        go_BaseUI.SetActive(false);
    }
}