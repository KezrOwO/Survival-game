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
    private Image legsSlotImage;

    [SerializeField]
    private Image feetSlotImage;

    // Garde une trace des équipements actuels
    private ItemData equipedHeadItem;    
    private ItemData equipedChestItem;    
    private ItemData equipedHandsItem;    
    private ItemData equipedLegsItem;    
    private ItemData equipedFeetItem;

    [SerializeField]
    private Button headSlotDesequipButton;

    [SerializeField]
    private Button chestSlotDesequipButton;

    [SerializeField]
    private Button handsSlotDesequipButton;

    [SerializeField]
    private Button legsSlotDesequipButton;

    [SerializeField]
    private Button feetSlotDesequipButton;

    private bool isOpen = false;

    public static Inventory instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseInventory();
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

        UpdateEquipementDesequipButtons();
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
            switch (itemCurrentlySelected.equipementType)
            {
                case EquipementType.Head:
                    DisablePreviousEquipedEquipment(equipedHeadItem);
                    headSlotImage.sprite = itemCurrentlySelected.visual;
                    equipedHeadItem = itemCurrentlySelected;
                    break;
                case EquipementType.Chest:
                    DisablePreviousEquipedEquipment(equipedChestItem);
                    chestSlotImage.sprite = itemCurrentlySelected.visual;
                    equipedChestItem = itemCurrentlySelected;
                    break;
                case EquipementType.Hands:
                    DisablePreviousEquipedEquipment(equipedHandsItem);
                    handsSlotImage.sprite = itemCurrentlySelected.visual;
                    equipedHandsItem = itemCurrentlySelected;
                    break;
                case EquipementType.Legs:
                    DisablePreviousEquipedEquipment(equipedLegsItem);
                    legsSlotImage.sprite = itemCurrentlySelected.visual;
                    equipedLegsItem = itemCurrentlySelected;
                    break;
                case EquipementType.Feet:
                    DisablePreviousEquipedEquipment(equipedFeetItem);
                    feetSlotImage.sprite = itemCurrentlySelected.visual;
                    equipedFeetItem = itemCurrentlySelected;
                    break;
            }

            for (int i = 0; i < equipementLibraryItem.elementsToDisable.Length; i++)
            {
                equipementLibraryItem.elementsToDisable[i].SetActive(false);
            }
            equipementLibraryItem.itemPrefab.SetActive(true);

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

    private void UpdateEquipementDesequipButtons()
    {
        headSlotDesequipButton.onClick.RemoveAllListeners();
        headSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Head); });
        headSlotDesequipButton.gameObject.SetActive(equipedHeadItem);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Chest); });
        chestSlotDesequipButton.gameObject.SetActive(equipedChestItem);

        handsSlotDesequipButton.onClick.RemoveAllListeners();
        handsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Hands); });
        handsSlotDesequipButton.gameObject.SetActive(equipedHandsItem);

        legsSlotDesequipButton.onClick.RemoveAllListeners();
        legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Legs); });
        legsSlotDesequipButton.gameObject.SetActive(equipedLegsItem);

        feetSlotDesequipButton.onClick.RemoveAllListeners();
        feetSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipement(EquipementType.Feet); });
        feetSlotDesequipButton.gameObject.SetActive(equipedFeetItem);
    }

    public void DesequipEquipement(EquipementType equipementType)
    {
        if (IsFull())
        {
            Debug.Log("Inventaire plein");
            return;
        }

        ItemData currentItem = null;

        switch (equipementType)
        {
            case EquipementType.Head:
                currentItem = equipedHeadItem;
                equipedHeadItem = null;
                headSlotImage.sprite = emptySlotVisual;
                break;
            case EquipementType.Chest:
                currentItem = equipedChestItem;
                equipedChestItem = null;
                chestSlotImage.sprite = emptySlotVisual;
                break;
            case EquipementType.Hands:
                currentItem = equipedHandsItem;
                equipedHandsItem = null;
                handsSlotImage.sprite = emptySlotVisual;
                break;
            case EquipementType.Legs:
                currentItem = equipedLegsItem;
                equipedLegsItem = null;
                legsSlotImage.sprite = emptySlotVisual;
                break;
            case EquipementType.Feet:
                currentItem = equipedFeetItem;
                equipedHeadItem = null;
                feetSlotImage.sprite = emptySlotVisual;
                break;
        }

        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == currentItem).First();

        if (equipementLibraryItem != null)
        {
            for (int i = 0; i < equipementLibraryItem.elementsToDisable.Length; i++)
            {
                equipementLibraryItem.elementsToDisable[i].SetActive(true);
            }
            equipementLibraryItem.itemPrefab.SetActive(false);
        }

        AddItem(currentItem);

        RefreshContent();
    }

    private void DisablePreviousEquipedEquipment(ItemData itemToDisable)
    {

        if(itemToDisable == null)
        {
            return;
        }

        EquipementLibraryItem equipementLibraryItem = equipementLibrary.content.Where(elem => elem.itemData == itemToDisable).First();

        if (equipementLibraryItem != null)
        {
            for (int i = 0; i < equipementLibraryItem.elementsToDisable.Length; i++)
            {
                equipementLibraryItem.elementsToDisable[i].SetActive(true);
            }
            equipementLibraryItem.itemPrefab.SetActive(false);
        }

        AddItem(itemToDisable);

    }
}
