using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionController : MonoBehaviour
{
    private bool pickupActivated = true;

    [SerializeField]
    private TextMeshProUGUI actionText; // 필요 컴포넌트

    [SerializeField]
    private InventoryUI inventory;

    [SerializeField]
    private StarterAssets.ThirdPersonController player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            string itemName = other.gameObject.name;
            if (pickupActivated)
                ItemInfoAppear(itemName);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("여기 들어오나");
                int isAcquired = inventory.AcquireItem(other.gameObject.GetComponent<ItemPickup>().item);
                if (isAcquired == 0)
                {
                    player.GetItem();
                    Destroy(other.gameObject);
                    ItemInfoDisappear();
                    pickupActivated = true;
                }
                if (isAcquired == 1)
                {
                    disappear();
                    ItemInfoAppear(other.gameObject.name + "can't pick it up");
                }
                if (isAcquired == 2)
                {
                    disappear();
                    ItemInfoAppear("Inventory is full");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            ItemInfoDisappear();
            pickupActivated = true;
        }
    }

    void disappear()
    {
        ItemInfoDisappear();
        pickupActivated = false;
    }

    private void ItemInfoAppear(string ItemName)
    {
        actionText.gameObject.SetActive(true);
        actionText.text = ItemName + " Pick up " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        actionText.gameObject.SetActive(false);
    }
}
