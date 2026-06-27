using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    private SkillData _skill;
    private Action<SkillData> _onClickCallback;

    public void Setup(SkillData skill, Action<SkillData> onClick)
    {
        _skill = skill;
        _onClickCallback = onClick;
        _nameText.text = skill.SkillName;
        GetComponent<Button>().onClick.AddListener(() => _onClickCallback?.Invoke(_skill));
    }
}