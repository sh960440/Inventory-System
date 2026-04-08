using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ConsumableSystem : MonoBehaviour
{
    private CharacterStats stats;

    void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }

    void OnEnable()
    {
        InventoryEvents.ItemConsumed += Apply;
    }

    void OnDisable()
    {
        InventoryEvents.ItemConsumed -= Apply;
    }

    void Apply(ConsumableData item)
    {
        // Instant effects
        foreach (var mod in item.InstantModifiers)
            stats.AddModifier(mod.Clone());

        // Duration effects
        if (item.DurationModifiers.Count > 0)
            StartCoroutine(ApplyDuration(item));
    }

    IEnumerator ApplyDuration(ConsumableData item)
    {
        var runtimeMods = new List<StatModifier>();

        foreach (var mod in item.DurationModifiers)
        {
            var clone = mod.Clone();
            runtimeMods.Add(clone);
            stats.AddModifier(clone);
        }

        yield return new WaitForSeconds(item.Duration);

        foreach (var mod in runtimeMods)
            stats.RemoveModifier(mod);
    }
}