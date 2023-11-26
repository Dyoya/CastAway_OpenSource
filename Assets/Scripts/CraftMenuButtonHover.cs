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
        // 초기 색상 저장
        originalColor1 = image1.color;
        originalColor2 = image2.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 올라가면 색상 변경
            image1.color = HoverColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 나가면 원래 색상으로 복원
            image1.color = originalColor1;
    }
}

