using System.Collections.Generic;
using UnityEngine;

public class DemoItemSpawner : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(10, 10);
    [SerializeField] private float spawnHeight = 3f;

    [Header("Random Items")]
    [SerializeField] private List<GameObject> randomItems;
    [SerializeField] private int randomItemCount = 20;

    [Header("Fixed Items")]
    [SerializeField] private List<GameObject> fixedItems;

    [Header("Pool")]
    [SerializeField] private ObjectPool pool;

    [Header("Container")]
    [SerializeField] private Transform itemContainer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(areaSize.x, 0.1f, areaSize.y));
    }

    public void SpawnItems()
    {
        if (pool == null || itemContainer == null || randomItems == null || randomItems.Count == 0)
            return;

        var points = GeneratePoints(25);
        int index = 0;

        for (int i = 0; i < randomItemCount && index < points.Count; i++)
        {
            var prefab = randomItems[Random.Range(0, randomItems.Count)];
            Spawn(prefab, points[index++]);
        }

        if (fixedItems == null)
            return;

        foreach (var prefab in fixedItems)
        {
            if (index >= points.Count)
                break;
            if (prefab != null)
                Spawn(prefab, points[index++]);
        }
    }

    public void ClearItems()
    {
        if (pool == null || itemContainer == null)
            return;

        var pickups = itemContainer.GetComponentsInChildren<ItemPickup>();

        foreach (var p in pickups)
        {
            var data = p.ItemData;
            if (data != null && data.WorldPrefab != null)
                pool.Return(data.WorldPrefab, p.gameObject);
        }
    }

    private void Spawn(GameObject prefab, Vector3 position)
    {
        var obj = pool.Get(prefab);
        obj.transform.SetParent(itemContainer);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    private List<Vector3> GeneratePoints(int count)
    {
        var points = new List<Vector3>();

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));
        float cellX = areaSize.x / gridSize;
        float cellZ = areaSize.y / gridSize;

        var cells = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++)
            for (int z = 0; z < gridSize; z++)
                cells.Add(new Vector2Int(x, z));

        for (int i = 0; i < cells.Count; i++)
        {
            int j = Random.Range(i, cells.Count);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }

        for (int i = 0; i < count && i < cells.Count; i++)
        {
            var c = cells[i];

            float x = -areaSize.x / 2 + (c.x + Random.value) * cellX;
            float z = -areaSize.y / 2 + (c.y + Random.value) * cellZ;

            points.Add(transform.position + new Vector3(x, spawnHeight, z));
        }

        return points;
    }
}