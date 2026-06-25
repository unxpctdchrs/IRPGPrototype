using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private GameObject _selectionCursor;
    [SerializeField] private Vector3 _cursorOffset = new Vector3(0, 1.5f, 0);

    public event Action<IBattler> OnTargetConfirmed;
    public event Action OnSelectionCanceled;

    private List<IBattler> _validTargets;
    private int _currentIndex = 0;

    private InputManager _inputManager;

    [Inject]
    public void Construct(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    private void Start()
    {
        _selectionCursor.SetActive(false);
    }

    public void StartSelection(List<IBattler> possibleTargets)
    {
        if (possibleTargets == null || possibleTargets.Count == 0) return;

        _validTargets = possibleTargets;
        _currentIndex = 0;
        
        _selectionCursor.SetActive(true);
        UpdateCursorPosition();

        _inputManager.OnMove += HandleNavigation;
        _inputManager.OnAttack += ConfirmTarget;
        _inputManager.OnCancel += CancelTargeting;
    }

    private void HandleNavigation(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f) return;

        if (direction.x > 0.1f) 
        {
            _currentIndex++;
            if (_currentIndex >= _validTargets.Count) _currentIndex = 0;
            UpdateCursorPosition();
        }
        else if (direction.x < -0.1f) 
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _validTargets.Count - 1;
            UpdateCursorPosition();
        }
    }

    private void UpdateCursorPosition()
    {
        MonoBehaviour targetObj = _validTargets[_currentIndex] as MonoBehaviour;
        
        if (targetObj != null)
        {
            _selectionCursor.transform.position = targetObj.transform.position + _cursorOffset;       
        }
    }

    private void ConfirmTarget()
    {
        UnplugInputs();
        _selectionCursor.SetActive(false);
        
        IBattler chosenTarget = _validTargets[_currentIndex];
        Debug.Log($"[TargetSelector] Target Confirmed: {((MonoBehaviour)chosenTarget).gameObject.name}");
        OnTargetConfirmed?.Invoke(chosenTarget);
    }

    private void CancelTargeting()
    {
        UnplugInputs();
        _selectionCursor.SetActive(false);
        OnSelectionCanceled?.Invoke();
    }

    private void UnplugInputs()
    {
        _inputManager.OnMove -= HandleNavigation;
        _inputManager.OnAttack -= ConfirmTarget;
        _inputManager.OnCancel -= CancelTargeting;
    }

    private void OnDisable()
    {
        if (_inputManager != null) UnplugInputs();
    }
}