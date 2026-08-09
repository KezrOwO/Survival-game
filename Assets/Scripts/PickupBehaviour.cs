using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PickupBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField] private Inventory inventory;

    [SerializeField]
    private MoveBehaviour playerMoveBehavior;

    private Item currentItem;

   public void DoPickup(Item item)
    {
        if (inventory.IsFull())
        {
            Debug.Log("Inventory full, can't pick up : " + item.name);
            return;
        }

        currentItem = item;
        playerAnimator.SetTrigger("Pickup");
        playerMoveBehavior.canMove = false;
        
    }

    public void addItemToInventory()
    {
        inventory.AddItem(currentItem.itemData);
        Destroy(currentItem.gameObject);

        currentItem = null;
    }

    public void ReEnablePlayerMovement()
    {
        playerMoveBehavior.canMove = true;

    }
}
