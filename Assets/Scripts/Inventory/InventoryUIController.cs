using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public GameObject inventoryPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
