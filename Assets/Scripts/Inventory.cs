using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Panel References")]

    [SerializeField]
    private List<ItemData> content = new List<ItemData>();

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotParent;

    const int InventorySize = 24;

    [SerializeField]
    private Transform dropPoint;

    [Header("Action Panel References")]

    [SerializeField]
    public GameObject actionPanel;

    [SerializeField]
    public GameObject useItemButton;

    [SerializeField]
    public GameObject equipItemButton;

    [SerializeField]
    public GameObject dropItemButton;

    [SerializeField]
    public GameObject destroyItemButton;

    private ItemData itemCurrentlySelected;

    [SerializeField]
    private Sprite emptySlotVisual;

    [Header("Equipement Panel References")]

    [SerializeField]
    private EquipementLibrary equipementLibrary;

    [SerializeField]
    private Image headSlotImage;

    [SerializeField]
    private Image chestSlotImage;

    [SerializeField]
    private Image handsSlotImage;

    [SerializeField]
    private Image legslotImage;

    [SerializeField]
    private Image feetSlotImage;

    private bool isOpen = false;

    public static Inventory instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RefreshContent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInvetory();
            }
        }
    }

    public void AddItem(ItemData item)
    {
        content.Add(item);
        RefreshContent();
    }

    private void OpenInvetory()
    {
        inventoryPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        actionPanel.SetActive(false);
        ToolTipSysteme.instance.Hide();
        isOpen = false;
    }

    private void RefreshContent()
    {
        //Clear all slot
        for (int i = 0; i < inventorySlotParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
        }

        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventorySlotParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = content[i];
            currentSlot.itemVisual.sprite = content[i].visual;
        }
    }

    public bool IsFull()
    {
        return InventorySize == content.Count;
    }

    public void OpenActionPanel(ItemData item, Vector3 slotPosition)
    {

        itemCurrentlySelected = item;

        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }


        switch (item.itemType)
        {
            case ItemType.Ressource:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(false);
                break;
            case ItemType.Equipment:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(true);
                break;
            case ItemType.Consumable:
                useItemButton.SetActive(true);
                equipItemButton.SetActive(false);
                break;
        }

        actionPanel.transform.position = slotPosition;
        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected = null;
    }


    public void UseActionButton()
    {
        print("Use Item : " + itemCurrentlySelected.name);
        CloseActionPanel();
    }
    public void EquipActionButton()
    {

        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == itemCurrentlySelected).First();

        if (equipementLibraryItem != null)
        {
            for (int i = 0; i < equipementLibraryItem.elementsToDisable.Length; i++)
            {
                equipementLibraryItem.elementsToDisable[i].SetActive(false);
            }
            equipementLibraryItem.itemPrefab.SetActive(true);

            switch (itemCurrentlySelected.equipementType)
            {
                case EquipementType.Head:
                    headSlotImage.sprite = itemCurrentlySelected.visual;
                    break;
                case EquipementType.Chest:
                    chestSlotImage.sprite = itemCurrentlySelected.visual;
                    break;
                case EquipementType.Hands:
                    handsSlotImage.sprite = itemCurrentlySelected.visual;
                    break;
                case EquipementType.Legs:
                    legslotImage.sprite = itemCurrentlySelected.visual;
                    break;
                case EquipementType.Feet:
                    feetSlotImage.sprite = itemCurrentlySelected.visual;
                    break;
            }

            content.Remove(itemCurrentlySelected);
            RefreshContent();
        }
        else
        {
            Debug.LogError("Equipement : " + itemCurrentlySelected.name + " non existant dans la librairie des équipements");
        }

        CloseActionPanel();
    }
    public void DropActionButton()
    {
        GameObject instantiatedItem = Instantiate(itemCurrentlySelected.prefab);
        instantiatedItem.transform.position = dropPoint.position;
        content.Remove(itemCurrentlySelected);
        RefreshContent();
        CloseActionPanel();
    }
    public void DestroyActionButton()
    {
        content.Remove(itemCurrentlySelected);
        RefreshContent();
        CloseActionPanel();
    }
}
