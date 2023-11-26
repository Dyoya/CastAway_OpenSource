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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Canvas�� ���� ���������� �Ѱ�, ���������� ��
            isCanvasActive = !isCanvasActive;
            canvas.enabled = isCanvasActive;

            // ���콺 ������ ���̱�/����� ����
            Cursor.visible = isCanvasActive;
        }
    }
}