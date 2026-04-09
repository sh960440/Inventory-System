using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demo/Equipment Visual Database")]
public class EquipmentVisualDatabase : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public string itemId;
        public GameObject prefab;
    }

    [SerializeField] private List<Entry> entries;

    private Dictionary<string, GameObject> _lookup;

    public GameObject GetPrefab(string itemId)
    {
        EnsureLookup();
        _lookup.TryGetValue(itemId, out var prefab);
        return prefab;
    }

    private void EnsureLookup()
    {
        if (_lookup != null)
            return;

        _lookup = new Dictionary<string, GameObject>();
        if (entries == null)
            return;

        foreach (var e in entries)
        {
            if (string.IsNullOrEmpty(e.itemId) || _lookup.ContainsKey(e.itemId))
                continue;
            _lookup.Add(e.itemId, e.prefab);
        }
    }
}