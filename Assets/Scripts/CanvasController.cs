using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Canvas canvas;
    private bool isCanvasActive = false;

    void Start()
    {
        // ���� ���� �� Canvas�� ������
        canvas.enabled = false;
    }

    void Update()
    {
        // E Ű�� ������ ��
        if (Input.GetKey(KeyCode.Tab))
        {
            // Canvas�� ���� ���������� �Ѱ�, ���������� ��
            isCanvasActive = true;
            canvas.enabled = isCanvasActive;

            // ���콺 ������ ���̱�/����� ����
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