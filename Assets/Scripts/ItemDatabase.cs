using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> items = new();

    private Dictionary<string, ItemData> lookup = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup.Clear();

        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (lookup.ContainsKey(item.Id))
            {
                Debug.LogError($"Duplicate Item ID detected: {item.Id}");
                continue;
            }

            lookup.Add(item.Id, item);
        }

        Debug.Log($"ItemDatabase initialized with {lookup.Count} items.");
    }

    public ItemData Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (lookup.TryGetValue(id, out var item))
            return item;

        Debug.LogWarning($"Item ID not found: {id}");
        return null;
    }
}