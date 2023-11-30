using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftMenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage image1;
    public RawImage image2;
    public RawImage image3;

    public Image tabBase1;
    public Image tabBase2;
    public Image tabBase3;

    public Color hover;

    private Color originalTab1Color;
    private Color originalTab2Color;
    private Color originalTab3Color;

    void Start()
    {
        originalTab1Color = tabBase1.color;
        originalTab2Color = tabBase2.color;
        originalTab3Color = tabBase3.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == image1.gameObject || eventData.pointerEnter == tabBase1.gameObject)
        {
            // ���콺�� image1 �Ǵ� tabBase1 ���� ���� ��
            tabBase1.color = hover;
        }
        else if (eventData.pointerEnter == image2.gameObject || eventData.pointerEnter == tabBase2.gameObject)
        {
            // ���콺�� image2 �Ǵ� tabBase2 ���� ���� ��
            tabBase2.color = hover;
        }
        else if (eventData.pointerEnter == image3.gameObject || eventData.pointerEnter == tabBase3.gameObject)
        {
            // ���콺�� image3 �Ǵ� tabBase3 ���� ���� ��
            tabBase3.color = hover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� UI ��Ҹ� ���� �� ��� �� ������ ������� ����
        tabBase1.color = originalTab1Color;
        tabBase2.color = originalTab2Color;
        tabBase3.color = originalTab3Color;
    }
}
