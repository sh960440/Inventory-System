using UnityEngine;

public class ItemSystemConfigurator : MonoBehaviour
{
    [SerializeField] private ItemSystemConfiguration config;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private Equipment equipment;

    private void Awake()
    {
        ApplyConfig();
    }

    private void ApplyConfig()
    {
        //inventory.ApplyConfig(config);
        //hotbar.ApplyConfig(config);
        //equipment.ApplyConfig(config);
    }
}