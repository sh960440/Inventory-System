using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 5;

    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab, transform);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}