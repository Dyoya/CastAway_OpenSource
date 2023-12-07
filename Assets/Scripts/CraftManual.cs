using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject slot1;
    [SerializeField]
    private GameObject slot2;
    [SerializeField]
    private GameObject slot3;

    [SerializeField]
    private InventoryUI inventory;

    public void SlotClick(int _slotNumber)
    {
        if (true)
        {
            //inventory.SlotHasItem(itemname, itemcount);
            go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
            go_Prefab = craft_fire[_slotNumber].go_prefab;
            isPreviewActivated = true;
            go_BaseUI.SetActive(false);
            isActivated = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    void Start()
    {
        go_BaseUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        if (isPreviewActivated)
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
        Cursor.visible = isActivated;
        if(Cursor.visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
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

    //��ư Ŭ���Ǹ� ����
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