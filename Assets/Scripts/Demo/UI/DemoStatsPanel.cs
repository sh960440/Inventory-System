using TMPro;
using UnityEngine;

public class DemoStatsPanel : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private CharacterStats characterStats;

    [Header("UI")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private TMP_Text spdText;

    private void OnEnable()
    {
        CharacterStats.OnStatsChanged += Refresh;
        CharacterStats.OnHPChanged += OnHPChanged;
        Refresh();
    }

    private void OnDisable()
    {
        CharacterStats.OnStatsChanged -= Refresh;
        CharacterStats.OnHPChanged -= OnHPChanged;
    }

    private void OnHPChanged(int _) => Refresh();

    private void Refresh()
    {
        if (characterStats == null)
            return;

        if (hpText != null)
            hpText.text = $"HP: {characterStats.CurrentHP} / {characterStats.GetMaxHP()}";
        if (atkText != null)
            atkText.text = $"ATK: {characterStats.GetFinalValue(StatType.Attack)}";
        if (defText != null)
            defText.text = $"DEF: {characterStats.GetFinalValue(StatType.Defense)}";
        if (spdText != null)
            spdText.text = $"SPD: {characterStats.GetFinalValue(StatType.MoveSpeed)}";
    }
}