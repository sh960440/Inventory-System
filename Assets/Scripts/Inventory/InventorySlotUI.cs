using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;
    public Image background;
    public Image iconImage;
    public TMP_Text countText;
    Inventory inventory;
    public ContextMenuUI contextMenuUI;
    public DraggableItemUI dragUI;
    public TooltipUI tooltipUI;
    bool isDragging = false;

    public void Setup(Inventory inv, int idx)
    {
        inventory = inv;
        slotIndex = idx;
    }

    public void Refresh()
    {
        var slot = inventory.slots[slotIndex];

        if (slot.item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";
        }
        else
        {
            iconImage.sprite = slot.item.icon;
            iconImage.enabled = true;
            iconImage.preserveAspect = true;

            countText.text = slot.item.stackable && slot.count > 1 
                ? slot.count.ToString() 
                : "";
        }
    }

    public void SetItem(Sprite sprite)
    {
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
        iconImage.preserveAspect = true;
    }

    public void Clear()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Drag only if left button is clicked
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (inventory.slots[slotIndex].item == null)
            return;

        isDragging = true;
        dragUI.SetSprite(inventory.slots[slotIndex].item.icon);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        dragUI.Hide();

        // Raycast UI to find which slot to drop
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            var other = r.gameObject.GetComponent<InventorySlotUI>();
            if (other != null)
            {
                DropOnto(other.slotIndex);
                return;
            }
        }
    }

    void Update()
    {
        if (isDragging)
            dragUI.FollowMouse();
    }

    void DropOnto(int targetIndex)
    {
        if (targetIndex == slotIndex) return;

        var a = inventory.slots[slotIndex];
        var b = inventory.slots[targetIndex];

        // If the same type and stackable
        if (a.item == b.item && a.item.stackable)
        {
            b.count += a.count;
            a.item = null;
            a.count = 0;
        }
        else
        {
            // swap
            var tempItem = a.item;
            var tempCount = a.count;

            a.item = b.item;
            a.count = b.count;

            b.item = tempItem;
            b.count = tempCount;
        }

        inventory.UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // If the slot is empty
            if (inventory.slots[slotIndex].item == null)
                return;

            // Shift + right click -> Split Stack
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                SplitStack();
            }
            else // Right Click -> Open Context Menu
            {
                // Open Context Menu
                contextMenuUI.ShowAt(eventData.position, inventory, slotIndex);
            }
        }
    }

    void SplitStack()
    {
        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;
        if (slot.count < 2) return;

        int half = slot.count / 2;

        // Find an empty slot
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            if (inventory.slots[i].item == null)
            {
                // fill in the slot
                inventory.slots[i].item = slot.item;
                inventory.slots[i].count = half;

                // original slot
                slot.count -= half;

                inventory.UpdateUI();
                return;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = new Color(1f, 1f, 1f, 0.9f);
        transform.localScale = Vector3.one * 1.05f;

        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        tooltipUI.Show(slot.item, slot.count);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = Color.white;
        transform.localScale = Vector3.one;

        tooltipUI.Hide();
    }
}
