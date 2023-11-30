using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabToSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage Image1;
    public RawImage Image2;
    public RawImage Image3;
    public Image tabBase1;
    public Image tabBase2;
    public Image tabBase3;
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;

    public Color hover;

    private Color originalTab1Color;
    private Color originalTab2Color;
    private Color originalTab3Color;



    void Start()
    {
        // 초기 상태로 Slot1은 활성화되고 Slot2, Slot3는 비활성화됨
        Slot1.SetActive(true);
        Slot2.SetActive(false);
        Slot3.SetActive(false);

        originalTab1Color = tabBase1.color;
        originalTab2Color = tabBase2.color;
        originalTab3Color = tabBase3.color;

        // Image1, Image2, Image3에 연결된 버튼에 클릭 이벤트를 추가
        Button image1Button = Image1.GetComponent<Button>();
        Button image2Button = Image2.GetComponent<Button>();
        Button image3Button = Image3.GetComponent<Button>();
        Button tabBase1Button = tabBase1.GetComponent<Button>();
        Button tabBase2Button = tabBase2.GetComponent<Button>();
        Button tabBase3Button = tabBase3.GetComponent<Button>();

        image1Button.onClick.AddListener(() => ToggleSlots(true, false, false));
        image2Button.onClick.AddListener(() => ToggleSlots(false, true, false));
        image3Button.onClick.AddListener(() => ToggleSlots(false, false, true));

        tabBase1Button.onClick.AddListener(() => ToggleSlots(true, false, false));
        tabBase2Button.onClick.AddListener(() => ToggleSlots(false, true, false));
        tabBase3Button.onClick.AddListener(() => ToggleSlots(false, false, true));
    }

    // Slot1, Slot2, Slot3의 활성/비활성을 토글하는 함수
    void ToggleSlots(bool showSlot1, bool showSlot2, bool showSlot3)// bool showSlot3)
    {
        Slot1.SetActive(showSlot1);
        Slot2.SetActive(showSlot2);
        Slot3.SetActive(showSlot3);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == Image1.gameObject || eventData.pointerEnter == tabBase1.gameObject)
        {
            // 마우스가 image1 또는 tabBase1 위에 있을 때
            tabBase1.color = hover;
        }
        else if (eventData.pointerEnter == Image2.gameObject || eventData.pointerEnter == tabBase2.gameObject)
        {
            // 마우스가 image2 또는 tabBase2 위에 있을 때
            tabBase2.color = hover;
        }
        else if (eventData.pointerEnter == Image3.gameObject || eventData.pointerEnter == tabBase3.gameObject)
        {
            // 마우스가 image3 또는 tabBase3 위에 있을 때
            tabBase3.color = hover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 UI 요소를 떠날 때 모든 탭 색상을 원래대로 복원
        tabBase1.color = originalTab1Color;
        tabBase2.color = originalTab2Color;
        tabBase3.color = originalTab3Color;
    }
}