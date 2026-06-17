using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAsset;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _pauseActionPlayer;
    private InputAction _pauseActionUI;
    private InputAction _interactAction;
    private InputAction _exitInteractionAction;
    private InputAction _attackAction;
    private InputAction _inventoryActionPlayer;
    private InputAction _inventoryActionUI;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool ExitInteractionTriggered { get; private set; }
    public bool PauseTriggeredPlayer { get; private set; }
    public bool PauseTriggeredUI { get; private set; }
    public int HotbarKeyPressed { get; private set; } = -1;
    public bool AttackTriggered { get; private set; }
    public bool InventoryTriggeredPlayer { get; private set; }
    public bool InventoryTriggeredUI { get; private set; }

    // [Inject] private readonly IPauseService _pauseService;
    // [Inject] private readonly IInventoryService _inventoryService;

    void Awake()
    {
        // _playerInteractor = GetComponent<PlayerInteractor>();

        var playerMap = _inputAsset.FindActionMap("Player");
        _moveAction = playerMap.FindAction("Move");
        _jumpAction = playerMap.FindAction("Jump");
        // _pauseActionPlayer = playerMap.FindAction("Pause");
        _lookAction = playerMap.FindAction("Look");
        // _attackAction = playerMap.FindAction("Attack");
        // _inventoryActionPlayer = playerMap.FindAction("Inventory");

        // var uiMap = _inputAsset.FindActionMap("UI");
        // _pauseActionUI = uiMap.FindAction("Pause");
        // _inventoryActionUI = uiMap.FindAction("Inventory");

        // var interactionMap = _inputAsset.FindActionMap("Interaction");
        // _interactAction = interactionMap.FindAction("Interact");
        // _exitInteractionAction = interactionMap.FindAction("ExitInteraction");
    }

    void Update()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        JumpTriggered = _jumpAction.WasPressedThisFrame();
        // PauseTriggeredPlayer = _pauseActionPlayer.WasPressedThisFrame();
        // PauseTriggeredUI = _pauseActionUI.WasPressedThisFrame();
        // InteractTriggered = _interactAction.WasPressedThisFrame();
        // ExitInteractionTriggered = _exitInteractionAction.WasPressedThisFrame();
        // AttackTriggered = _attackAction.WasPressedThisFrame();
        // InventoryTriggeredPlayer = _inventoryActionPlayer.WasPressedThisFrame();
        // InventoryTriggeredUI = _inventoryActionUI.WasPressedThisFrame();

        // CheckForPause();
        // CheckInventoryTriggered();
        // CheckBottomInventoryKeypadTriggered();
    }

    void OnEnable()
    {
        _inputAsset.FindActionMap("Player").Enable();
        // _inputAsset.FindActionMap("Interaction").Enable();
    }

    void OnDisable()
    {
        _inputAsset.FindActionMap("Player").Disable();
    }

    // private void CheckForPause()
    // {
    //     if (_playerInteractor.IsInteracting) return;

    //     bool pausePressed = PauseTriggeredPlayer || PauseTriggeredUI;
    //     if (pausePressed)
    //     {
    //         _pauseService.TogglePause();
    //         EnablePlayerActionMap(!_pauseService.IsPaused);
    //         EnableUIActionMap(_pauseService.IsPaused);   
    //     }
    // }

    // private void CheckInventoryTriggered()
    // {
    //     bool inventoryPressed = InventoryTriggeredPlayer || InventoryTriggeredUI;
    //     if (inventoryPressed)
    //     {
    //         _inventoryService.ToggleInventory();
    //         EnablePlayerActionMap(!_inventoryService.IsInventoryOpened);
    //         EnableUIActionMap(_inventoryService.IsInventoryOpened);   
    //     }
    // }

    public void EnablePlayerActionMap(bool isEnabled)
    {
        if (isEnabled)
        {
            _inputAsset.FindActionMap("Player").Enable();
        }
        else
        {
            _inputAsset.FindActionMap("Player").Disable();
        }
    }

    public void EnableUIActionMap(bool isEnabled)
    {
        if (isEnabled)
        {
            _inputAsset.FindActionMap("UI").Enable();
        }
        else
        {
            _inputAsset.FindActionMap("UI").Disable();
        }
    }

    public void EnableInteractionActionMap(bool isEnabled)
    {
        if (isEnabled)
        {
            _inputAsset.FindActionMap("Interaction").Enable();
        }
        else
        {
            _inputAsset.FindActionMap("Interaction").Disable();
        }
    }

    private void CheckBottomInventoryKeypadTriggered()
    {
        HotbarKeyPressed = -1;
        for (int i = 0; i < 9; i++)
        {
            if (Keyboard.current[Key.Digit1 + i].wasPressedThisFrame)
            {
                HotbarKeyPressed = i;
                break;
            }
        }
    }
}