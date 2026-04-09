using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    [SerializeField] private EffectPool pool;

    public void Spawn(string text, Vector3 position)
    {
        if (pool == null)
            return;

        var obj = pool.Get();
        obj.transform.position = position;

        var floating = obj.GetComponent<FloatingText>();
        if (floating != null)
        {
            floating.SetText(text);
            floating.Init(pool);
        }
    }
}