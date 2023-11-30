using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabToSlot : MonoBehaviour
{
    public RawImage Image1;
    public RawImage Image2;
    public RawImage Image3;
    public Image SlotBase1;
    public Image SlotBase2;
    public Image SlotBase3;
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;

    public Color hover;

    private Color originalTab1Color;
    private Color originalTab2Color;
    private Color originalTab3Color;



    void Start()
    {
        // �ʱ� ���·� Slot1�� Ȱ��ȭ�ǰ� Slot2, Slot3�� ��Ȱ��ȭ��
        Slot1.SetActive(true);
        Slot2.SetActive(false);
        Slot3.SetActive(false);

        originalTab1Color = SlotBase1.color;
        originalTab2Color = SlotBase2.color;
        originalTab3Color = SlotBase3.color;

        // Image1, Image2, Image3�� ����� ��ư�� Ŭ�� �̺�Ʈ�� �߰�
        Button image1Button = Image1.GetComponent<Button>();
        Button image2Button = Image2.GetComponent<Button>();
        Button image3Button = Image3.GetComponent<Button>();
        Button tabbase1Button = SlotBase1.GetComponent<Button>();
        Button tabbase2Button = SlotBase2.GetComponent<Button>();
        Button tabbase3Button = SlotBase3.GetComponent<Button>();

        image1Button.onClick.AddListener(() => ToggleSlots(true, false, false));
        image2Button.onClick.AddListener(() => ToggleSlots(false, true, false));
        image3Button.onClick.AddListener(() => ToggleSlots(false, false, true));

        tabbase1Button.onClick.AddListener(() => ToggleSlots(true, false, false));
        tabbase2Button.onClick.AddListener(() => ToggleSlots(false, true, false));
        tabbase3Button.onClick.AddListener(() => ToggleSlots(false, false, true));
    }

    // Slot1, Slot2, Slot3�� Ȱ��/��Ȱ���� ����ϴ� �Լ�
    void ToggleSlots(bool showSlot1, bool showSlot2, bool showSlot3)// bool showSlot3)
    {
        Slot1.SetActive(showSlot1);
        Slot2.SetActive(showSlot2);
        Slot3.SetActive(showSlot3);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == Image1.gameObject || eventData.pointerEnter == SlotBase1.gameObject)
        {
            // ���콺�� image1 �Ǵ� tabBase1 ���� ���� ��
            SlotBase1.color = hover;
        }
        else if (eventData.pointerEnter == Image2.gameObject || eventData.pointerEnter == SlotBase2.gameObject)
        {
            // ���콺�� image2 �Ǵ� tabBase2 ���� ���� ��
            SlotBase2.color = hover;
        }
        else if (eventData.pointerEnter == Image3.gameObject || eventData.pointerEnter == SlotBase3.gameObject)
        {
            // ���콺�� image3 �Ǵ� tabBase3 ���� ���� ��
            SlotBase3.color = hover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� UI ��Ҹ� ���� �� ��� �� ������ ������� ����
        SlotBase1.color = originalTab1Color;
        SlotBase2.color = originalTab2Color;
        SlotBase3.color = originalTab3Color;
    }
}