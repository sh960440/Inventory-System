using UnityEngine;

public class HotbarInput : MonoBehaviour
{
    public Hotbar hotbar;

    [Header("Key Bindings")]
    public KeyCode[] keys;

    void Awake()
    {
        if (keys == null || keys.Length == 0)
        {
            keys = new KeyCode[hotbar.slots.Count];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = KeyCode.Alpha1 + i;
        }
    }

    void Update()
    {
        for (int i = 0; i < keys.Length && i < hotbar.slots.Count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
                TriggerHotbar(i);
        }
    }

    void TriggerHotbar(int index)
    {
        if (!hotbar.ValidHotbarIndex(index))
        {
            hotbar.Clear(index);
            return;
        }

        var invSlot = hotbar.GetInventorySlot(index);
        if (invSlot == null || invSlot.item == null)
        {

            hotbar.Clear(index);
            return;
        }

        GameEvents.OnHotbarUseRequested?.Invoke(invSlot);
    }
}