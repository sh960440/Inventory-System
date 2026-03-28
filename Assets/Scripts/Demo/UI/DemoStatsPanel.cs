using UnityEngine;
using TMPro;

public class DemoStatsPanel : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;

    [Header("UI")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private TMP_Text spdText;

    void OnEnable()
    {
        CharacterStats.OnStatsChanged += Refresh;
        CharacterStats.OnHPChanged += OnHPChanged;
        Refresh();
    }

    void OnDisable()
    {
        CharacterStats.OnStatsChanged -= Refresh;
        CharacterStats.OnHPChanged -= OnHPChanged;
    }

    void OnHPChanged(int _) => Refresh();

    private void Refresh()
    {
        hpText.text = $"HP: {characterStats.CurrentHP} / {characterStats.GetMaxHP()}";
        atkText.text = $"ATK: {characterStats.GetFinalValue(StatType.Attack)}";
        defText.text = $"DEF: {characterStats.GetFinalValue(StatType.Defense)}";
        spdText.text = $"SPD: {characterStats.GetFinalValue(StatType.MoveSpeed)}";
    }
}