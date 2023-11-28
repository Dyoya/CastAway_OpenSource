using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftMenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image1;
    public RawImage image2;
    public Color HoverColor;
    private Color originalColor1;
    private Color originalColor2;

    void Start()
    {
        // �ʱ� ���� ����
        originalColor1 = image1.color;
        originalColor2 = image2.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� �ö󰡸� ���� ����
            image1.color = HoverColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� ������ ���� �������� ����
            image1.color = originalColor1;
    }
}

