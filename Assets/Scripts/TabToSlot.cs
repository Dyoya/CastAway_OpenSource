using UnityEngine;
using UnityEngine.UI;

public class TabToSlot : MonoBehaviour
{
    public RawImage Image1;
    public RawImage Image2;
    //public Image Image3;
    public GameObject Slot1;
    public GameObject Slot2;
   // public GameObject Slot3;

    void Start()
    {
        // �ʱ� ���·� Slot1�� Ȱ��ȭ�ǰ� Slot2, Slot3�� ��Ȱ��ȭ��
        Slot1.SetActive(true);
        Slot2.SetActive(false);
        //Slot3.SetActive(false);

        // Image1, Image2, Image3�� ����� ��ư�� Ŭ�� �̺�Ʈ�� �߰�
        Button image1Button = Image1.GetComponent<Button>();
        Button image2Button = Image2.GetComponent<Button>();
        //Button image3Button = Image3.GetComponent<Button>();

        image1Button.onClick.AddListener(() => ToggleSlots(true, false));
        image2Button.onClick.AddListener(() => ToggleSlots(false, true));
        //image3Button.onClick.AddListener(() => ToggleSlots(false, false, true));
    }

    // Slot1, Slot2, Slot3�� Ȱ��/��Ȱ���� ����ϴ� �Լ�
    void ToggleSlots(bool showSlot1, bool showSlot2)// bool showSlot3)
    {
        Slot1.SetActive(showSlot1);
        Slot2.SetActive(showSlot2);
        //Slot3.SetActive(showSlot3);
    }
}