using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input Asset")]
    [SerializeField] private InputActionAsset _inputAsset;

    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnPauseToggle;
    public event Action<int> OnHotbarSelected;
    public event Action OnCancel;

    private InputActionMap _playerMap;
    private InputActionMap _uiMap;

    private InputAction _moveAction, _jumpAction, _attackAction, _pauseAction, _cancelAction, _interactAction;

    private void Awake()
    {
        _playerMap = _inputAsset.FindActionMap("Player");
        _uiMap = _inputAsset.FindActionMap("UI");

        _moveAction = _playerMap.FindAction("Move");
        _jumpAction = _playerMap.FindAction("Jump");
        _attackAction = _playerMap.FindAction("Attack");
        _interactAction = _playerMap.FindAction("Interact");
        _cancelAction = _uiMap.FindAction("Cancel");
        _pauseAction = _playerMap.FindAction("Pause");

        _moveAction.performed += _ => OnMove?.Invoke(_.ReadValue<Vector2>());
        _moveAction.canceled += _ => OnMove?.Invoke(Vector2.zero);

        _jumpAction.performed += _ => OnJump?.Invoke();
        _attackAction.performed += _ => OnAttack?.Invoke();
        _interactAction.performed += _ => OnInteract?.Invoke();
        _cancelAction.performed += _ => OnCancel?.Invoke();
        _pauseAction.performed += _ => OnPauseToggle?.Invoke();
    }

    private void OnEnable()
    {
        _playerMap.Enable();
    }

    private void OnDisable()
    {
        _playerMap.Disable();
        _uiMap.Disable();
    }
    
    public void EnablePlayerControls()
    {
        _uiMap.Disable();
        _playerMap.Enable();
    }

    public void EnableUIControls()
    {
        _playerMap.Disable();
        _uiMap.Enable();
    }
}