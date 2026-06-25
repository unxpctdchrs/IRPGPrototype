using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionMenuPanel;
    [SerializeField] private Button _attackButton;
    [SerializeField] private TargetSelector _targetSelector;
    [SerializeField] private BattleSetup _battleSetup;

    void Start()
    {
        _attackButton.onClick.AddListener(OnAttackButtonClicked);
        _targetSelector.OnSelectionCanceled += OpenActionMenu;
        OpenActionMenu();
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
}