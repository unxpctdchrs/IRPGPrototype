using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Action HUD")]
    [SerializeField] private GameObject _actionMenuPanel;
    [SerializeField] private Button _attackButton;
    [SerializeField] private TargetSelector _targetSelector;
    [SerializeField] private BattleSetup _battleSetup;

    [Header("Party HUD")]
    [SerializeField] private Transform _partyInfoContainer; 
    [SerializeField] private CharacterInfoUI _characterInfoPrefab;

    void Start()
    {
        _attackButton.onClick.AddListener(OnAttackButtonClicked);
        _targetSelector.OnSelectionCanceled += OpenActionMenu;
        OpenActionMenu();
    }

    void Update()
    {
        if (_actionMenuPanel.activeInHierarchy && EventSystem.current.currentSelectedGameObject == null)
        {
            // force to grab the attack button again
            EventSystem.current.SetSelectedGameObject(_attackButton.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_targetSelector != null) 
        {
            _targetSelector.OnSelectionCanceled -= OpenActionMenu;
        }
    }

    public void OpenActionMenu()
    {
        _actionMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_attackButton.gameObject);
    }

    public void OnAttackButtonClicked()
    {
        _actionMenuPanel.SetActive(false);
        List<IBattler> livingEnemies = _battleSetup.GetActiveEnemies();
        _targetSelector.StartSelection(livingEnemies);
    }

    public HealthBar SpawnCharacterInfoUI(CharacterData data)
    {
        CharacterInfoUI newCharacterInfo = Instantiate(_characterInfoPrefab, _partyInfoContainer);
        newCharacterInfo.SetupUI(data);
        return newCharacterInfo.GetHealthBar();
    }
}