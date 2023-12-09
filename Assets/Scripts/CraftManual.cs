using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    private bool isActivated = false;  // CraftManual UI 활성 상태
    private bool isPreviewActivated = false; // 미리 보기 활성화 상태

    [SerializeField]
    private GameObject go_BaseUI; // 기본 베이스 UI

    [SerializeField]
    private Craft[] craft_fire; //불 탭에 있는 슬롯들. 
    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    [SerializeField]
    private Transform tf_Player;  // 플레이어 위치

    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //동현이가 추가함
    [SerializeField]
    private GameObject[] slots;

    [SerializeField]
    private InventoryUI inventory;

    [SerializeField]
    private StarterAssets.ThirdPersonController thePlayer;

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
            string[] lines = ExplainText.text.Split('\n'); // 텍스트를 줄바꿈 문자를 기준으로 분리

            Dictionary<string, int> materials = new Dictionary<string, int>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(' '); // 텍스트를 공백을 기준으로 분리

                string itemName = parts[0];
                int itemCount = int.Parse(parts[1]);

                // 딕셔너리에 재료 추가
                materials[itemName] = itemCount;
            }

            bool hasAllMaterials = true; // 모든 재료가 있어야됨
            foreach (KeyValuePair<string, int> material in materials)
            {
                Debug.Log(material.Key);
                Debug.Log(material.Value);
                if (!inventory.SlotHasItem(material.Key, material.Value))
                {
                    hasAllMaterials = false;
                    break;
                }
            }

            if (hasAllMaterials && _slotNumber <= 2)
            {
                Debug.Log("빌드 실행");
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
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
            GameManager.isPause = !GameManager.isPause;
        }

        if (isPreviewActivated)
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
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

                Debug.Log(_location);
                Debug.Log(go_Preview.transform.position);
            }
            else
            {
                go_Preview.SetActive(false);
            }
        }
        else
            go_Preview.SetActive(false);
    }

    //버튼 클릭되면 실행
    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            Instantiate(go_Prefab, hitInfo.point, Quaternion.identity);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }
    //Tab키 누르면 실행
    private void Window()
    {   
        if (!isActivated) //창 닫혀있으면 창을 킴
            OpenWindow();
        else
            CloseWindow(); // 창 열려있으면 창을 닫음
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
    //ESC누르면 실행
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