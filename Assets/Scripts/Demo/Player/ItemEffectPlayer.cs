using System.Collections;
using System.Linq;
using UnityEngine;

public class ItemEffectPlayer : MonoBehaviour
{
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private GameObject buffEffectPrefab;
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private FloatingTextSpawner floatingTextSpawner;
    [SerializeField] private EffectPool healEffectPool;
    [SerializeField] private EffectPool buffEffectPool;

    void OnEnable()
    {
        InventoryEvents.ItemConsumed += PlayEffect;
    }

    void OnDisable()
    {
        InventoryEvents.ItemConsumed -= PlayEffect;
    }

    void PlayEffect(ConsumableData item)
    {
        if (item.InstantModifiers.Any(m => m.StatType == StatType.Health))
        {
            var effect = healEffectPool.Get();
            effect.transform.position = effectSpawnPoint.position;

            var ps = effect.GetComponent<ParticleSystem>();
            ps.Play();

            StartCoroutine(ReturnAfter(ps.main.duration, effect, healEffectPool));

            floatingTextSpawner.Spawn("+HP", effectSpawnPoint.position);
        }

        if (item.DurationModifiers.Count > 0)
        {
            var effect = buffEffectPool.Get();
            effect.transform.position = effectSpawnPoint.position;

            var ps = effect.GetComponent<ParticleSystem>();
            ps.Play();

            StartCoroutine(ReturnAfter(ps.main.duration, effect, buffEffectPool));
        }
    }

    private IEnumerator ReturnAfter(float time, GameObject obj, EffectPool pool)
    {
        yield return new WaitForSeconds(time);
        pool.Return(obj);
    }
}