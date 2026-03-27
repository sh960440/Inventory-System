using System.Collections.Generic;
using UnityEngine;

public class DemoItemSpawner : MonoBehaviour
{
    [Header("Area")]
    public Vector2 areaSize = new Vector2(10, 10);
    public float spawnHeight = 3f;

    [Header("Random Items")]
    public List<GameObject> randomItems;
    public int randomItemCount = 20;

    [Header("Fixed Items")]
    public List<GameObject> fixedItems;

    [Header("Pool")]
    public ObjectPool pool;

    [Header("Container")]
    [SerializeField] private Transform itemContainer;

    public void SpawnItems()
    {
        var points = GeneratePoints(25);

        int index = 0;

        // random items
        for (int i = 0; i < randomItemCount && index < points.Count; i++)
        {
            var prefab = randomItems[Random.Range(0, randomItems.Count)];
            Spawn(prefab, points[index++]);
        }

        // fixed weapons
        foreach (var prefab in fixedItems)
        {
            if (index >= points.Count) break;
            Spawn(prefab, points[index++]);
        }
    }

    void Spawn(GameObject prefab, Vector3 position)
    {
        var obj = pool.Get(prefab);
        obj.transform.SetParent(itemContainer);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    List<Vector3> GeneratePoints(int count)
    {
        List<Vector3> points = new();

        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        float cellX = areaSize.x / gridSize;
        float cellZ = areaSize.y / gridSize;

        List<Vector2Int> cells = new();

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

            Vector3 pos =
                transform.position + new Vector3(x, spawnHeight, z);

            points.Add(pos);
        }

        return points;
    }

    public void ClearItems()
    {
        var pickups = itemContainer.GetComponentsInChildren<ItemPickup>();

        foreach (var p in pickups)
        {
            pool.Return(p.itemData.worldPrefab, p.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(areaSize.x, 0.1f, areaSize.y));
    }
}