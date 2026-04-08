using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Provides a lookup database for item definitions, resolving items by their unique IDs.
/// </summary>
public class ItemDatabase : MonoBehaviour, IItemDatabase
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Definitions")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private readonly Dictionary<string, ItemData> _lookup = new Dictionary<string, ItemData>();

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public ItemData Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (_lookup.TryGetValue(id, out var item))
            return item;

        Debug.LogWarning($"Item ID not found: {id}");
        return null;
    }

    private void BuildLookup()
    {
        _lookup.Clear();

        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (_lookup.ContainsKey(item.Id))
            {
                Debug.LogError($"Duplicate Item ID detected: {item.Id}");
                continue;
            }

            _lookup.Add(item.Id, item);
        }
    }
}