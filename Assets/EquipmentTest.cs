using UnityEngine;

public class EquipmentTest : MonoBehaviour
{
    public EquipmentData testEquipment;

    CharacterStats stats;

    void Start()
    {
        stats = FindFirstObjectByType<CharacterStats>();

        Debug.Log("=== BEFORE EQUIP ===");
        LogStats();

        // UI / Inventory sends a OnEquipRequested
        GameEvents.OnEquipRequested?.Invoke(testEquipment);

        Debug.Log("=== AFTER EQUIP ===");
        LogStats();

        // Delay for 1 second and upquip.
        Invoke(nameof(TestUnequip), 1.5f);
    }

    void TestUnequip()
    {
        Debug.Log("=== BEFORE UNEQUIP ===");
        LogStats();

        GameEvents.OnUnequipRequested?.Invoke(testEquipment.equipSlot);

        Debug.Log("=== AFTER UNEQUIP ===");
        LogStats();
    }

    void LogStats()
    {
        Debug.Log($"Attack = {stats.GetFinalValue(StatType.Attack)}");
        //Debug.Log($"Defense = {stats.GetFinalValue(StatType.Defense)}");
        //Debug.Log($"MoveSpeed = {stats.GetFinalValue(StatType.MoveSpeed)}");
    }
}