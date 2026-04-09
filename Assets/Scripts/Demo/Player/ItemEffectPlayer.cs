using System.Collections;
using System.Linq;
using UnityEngine;

public class ItemEffectPlayer : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private FloatingTextSpawner floatingTextSpawner;
    [SerializeField] private EffectPool healEffectPool;
    [SerializeField] private EffectPool buffEffectPool;

    private void OnEnable()
    {
        InventoryEvents.ItemConsumed += PlayEffect;
    }

    private void OnDisable()
    {
        InventoryEvents.ItemConsumed -= PlayEffect;
    }

    private void PlayEffect(ConsumableData item)
    {
        if (item == null || effectSpawnPoint == null)
            return;

        if (item.InstantModifiers.Any(m => m.StatType == StatType.Health) && healEffectPool != null)
        {
            var effect = healEffectPool.Get();
            effect.transform.position = effectSpawnPoint.position;

            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                StartCoroutine(ReturnAfter(ps.main.duration, effect, healEffectPool));
            }

            floatingTextSpawner?.Spawn("+HP", effectSpawnPoint.position);
        }

        if (item.DurationModifiers.Count > 0 && buffEffectPool != null)
        {
            var effect = buffEffectPool.Get();
            effect.transform.position = effectSpawnPoint.position;

            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                StartCoroutine(ReturnAfter(ps.main.duration, effect, buffEffectPool));
            }
        }
    }

    private IEnumerator ReturnAfter(float time, GameObject obj, EffectPool pool)
    {
        yield return new WaitForSeconds(time);
        pool.Return(obj);
    }
}