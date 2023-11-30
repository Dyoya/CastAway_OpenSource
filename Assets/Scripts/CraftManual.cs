using System.Collections;
using System.Collections.Generic;
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


    public void SlotClick(int _slotNumber)
    {
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_prefab;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
        isActivated = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

  
    private void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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