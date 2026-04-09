using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Motion")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lifetime = 1.2f;

    private float _timer;
    private Color _originalColor;
    private EffectPool _pool;

    private void Awake()
    {
        if (text != null)
            _originalColor = text.color;
    }

    public void Init(EffectPool poolRef)
    {
        _pool = poolRef;
        _timer = 0f;
        if (text != null)
            text.color = _originalColor;
    }

    public void SetText(string value)
    {
        if (text != null)
            text.text = value;
    }

    private void Update()
    {
        if (text == null || _pool == null)
            return;

        // Move up
        transform.position += Vector3.up * (moveSpeed * Time.deltaTime);

        // Fade out
        _timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, _timer / lifetime);
        text.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, alpha);

        if (_timer >= lifetime)
            _pool.Return(gameObject);
    }
}