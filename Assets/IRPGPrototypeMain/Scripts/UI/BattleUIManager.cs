using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Action HUD")]
    [SerializeField] private GameObject _actionMenuPanel;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _skillButton;
    [SerializeField] private TurnBaseController _turnBaseController;
    [SerializeField] private BattleSetup _battleSetup;

    [Header("Action Skill HUD")]
    [SerializeField] private GameObject _skillMenuPanel;
    [SerializeField] private Transform _skillButtonContainer;
    [SerializeField] private SkillButton _skillButtonPrefab;

    [Header("Party HUD")]
    [SerializeField] private Transform _partyInfoContainer; 
    [SerializeField] private CharacterInfoUI _characterInfoPrefab;

    private List<SkillData> _currentCharacterSkills;

    void Start()
    {
        _attackButton.onClick.AddListener(OnAttackButtonClicked);
        _skillButton.onClick.AddListener(() => OnSkillButtonClicked(_currentCharacterSkills, OnSkillSelected));
        if (_turnBaseController == null) _turnBaseController = FindFirstObjectByType<TurnBaseController>();
        CloseActionMenu();
    }

    void Update()
    {
        if (_actionMenuPanel.activeInHierarchy && EventSystem.current.currentSelectedGameObject == null)
        {
            // force to grab the attack button again
            EventSystem.current.SetSelectedGameObject(_attackButton.gameObject);
        }
    }

    public void OpenActionMenu()
    {
        StartCoroutine(OpenMenuRoutine());
    }

    private IEnumerator OpenMenuRoutine()
    {
        yield return null; 

        _actionMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_attackButton.gameObject);
    }

    public void CloseActionMenu()
    {
        _actionMenuPanel.SetActive(false);
    }

    public void OnAttackButtonClicked()
    {
        _actionMenuPanel.SetActive(false);
        _turnBaseController.StartEnemySelection();
    }

    public HealthBar SpawnCharacterInfoUI(CharacterData data)
    {
        CharacterInfoUI newCharacterInfo = Instantiate(_characterInfoPrefab, _partyInfoContainer);
        newCharacterInfo.SetupUI(data);
        return newCharacterInfo.GetHealthBar();
    }

    public void OnSkillButtonClicked(List<SkillData> skills, Action<SkillData> onSkillSelected)
    {
        foreach (Transform child in _skillButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var skill in skills)
        {
            SkillButton newBtn = Instantiate(_skillButtonPrefab, _skillButtonContainer);
            newBtn.Setup(skill, onSkillSelected);
        }

        _skillMenuPanel.SetActive(true);
    }

    public void UpdateActionMenuSkillButton(List<SkillData> skills)
    {
        _currentCharacterSkills = skills;
        bool hasSkills = skills != null && skills.Count > 0;
        _skillButton.interactable = hasSkills;
    }

    private void OnSkillSelected(SkillData skill)
    {
        _skillMenuPanel.SetActive(false);
        _actionMenuPanel.SetActive(false);
        _turnBaseController.OnSkillChosen(skill);
    }

    public void CloseSkillMenu()
    {
        _skillMenuPanel.SetActive(false);
    }
}