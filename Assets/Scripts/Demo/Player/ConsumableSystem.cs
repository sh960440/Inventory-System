using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableSystem : MonoBehaviour
{
    private CharacterStats _stats;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();
    }

    private void OnEnable()
    {
        InventoryEvents.ItemConsumed += Apply;
    }

    private void OnDisable()
    {
        InventoryEvents.ItemConsumed -= Apply;
    }

    private void Apply(ConsumableData item)
    {
        if (_stats == null || item == null)
            return;

        // Instant effects
        foreach (var mod in item.InstantModifiers)
            _stats.AddModifier(mod.Clone());

        // Duration effects
        if (item.DurationModifiers.Count > 0)
            StartCoroutine(ApplyDuration(item));
    }

    private IEnumerator ApplyDuration(ConsumableData item)
    {
        var runtimeMods = new List<StatModifier>();

        foreach (var mod in item.DurationModifiers)
        {
            var clone = mod.Clone();
            runtimeMods.Add(clone);
            _stats.AddModifier(clone);
        }

        yield return new WaitForSeconds(item.Duration);

        foreach (var mod in runtimeMods)
            _stats.RemoveModifier(mod);
    }
}