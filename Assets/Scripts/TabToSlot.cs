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
        // �ʱ� ���·� Slot1�� Ȱ��ȭ�ǰ� Slot2, Slot3�� ��Ȱ��ȭ��
        Slot1.SetActive(true);
        Slot2.SetActive(false);
        Slot3.SetActive(false);

        originalTab1Color = tabBase1.color;
        originalTab2Color = tabBase2.color;
        originalTab3Color = tabBase3.color;

        // Image1, Image2, Image3�� ����� ��ư�� Ŭ�� �̺�Ʈ�� �߰�
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

    // Slot1, Slot2, Slot3�� Ȱ��/��Ȱ���� ����ϴ� �Լ�
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
            // ���콺�� image1 �Ǵ� tabBase1 ���� ���� ��
            tabBase1.color = hover;
        }
        else if (eventData.pointerEnter == Image2.gameObject || eventData.pointerEnter == tabBase2.gameObject)
        {
            // ���콺�� image2 �Ǵ� tabBase2 ���� ���� ��
            tabBase2.color = hover;
        }
        else if (eventData.pointerEnter == Image3.gameObject || eventData.pointerEnter == tabBase3.gameObject)
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