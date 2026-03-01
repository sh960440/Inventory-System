using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lifetime = 1.2f;

    private float timer;
    private Color originalColor;

    private EffectPool pool;

    private void Awake()
    {
        originalColor = text.color;
    }

    public void Init(EffectPool poolRef)
    {
        pool = poolRef;
        timer = 0f;
        text.color = originalColor;
    }

    public void SetText(string value)
    {
        text.text = value;
    }

    private void Update()
    {
        // Move up
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Timer
        timer += Time.deltaTime;

        // Fade out
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= lifetime)
        {
            pool.Return(gameObject);
        }
    }
}