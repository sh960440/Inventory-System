using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    [SerializeField] private EffectPool pool;

    public void Spawn(string text, Vector3 position)
    {
        var obj = pool.Get();

        obj.transform.position = position;

        var floating = obj.GetComponent<FloatingText>();
        floating.SetText(text);
        floating.Init(pool);
    }
}