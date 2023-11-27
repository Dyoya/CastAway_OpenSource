using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Canvas canvas;
    private bool isCanvasActive = false;

    void Start()
    {
        // 게임 시작 시 Canvas를 꺼놓음
        canvas.enabled = false;
    }

    void Update()
    {
        // E 키를 눌렀을 때
        if (Input.GetKey(KeyCode.Tab))
        {
            // Canvas가 현재 꺼져있으면 켜고, 켜져있으면 끔
            isCanvasActive = true;
            canvas.enabled = isCanvasActive;

            // 마우스 포인터 보이기/숨기기 설정
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = isCanvasActive;
        }
        else if(!Input.GetKey(KeyCode.Tab))
        {
            isCanvasActive = false;
            canvas.enabled = isCanvasActive;
            Cursor.visible = isCanvasActive;
        }
    }
}