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

    private Dictionary<string, GameObject> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<string, GameObject>();

        foreach (var e in entries)
        {
            if (!lookup.ContainsKey(e.itemId))
                lookup.Add(e.itemId, e.prefab);
        }
    }

    public GameObject GetPrefab(string itemId)
    {
        if (lookup == null)
            Initialize();

        lookup.TryGetValue(itemId, out var prefab);
        return prefab;
    }
}