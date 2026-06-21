using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public bool IsInteracting => _interactableStored != null;
    [SerializeField] private GameObject _interactUI;
    private PlayerInputHandler _playerInputHandler;
    private PlayerMovement _playerMovement;
    private IInteractable _interactableStored;
    private IInteractable _lookedAtInteractable;
    private float _rayLength = 6f;
    private TextMeshProUGUI _interactUITmp;
    private Image _icon;

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerMovement = GetComponent<PlayerMovement>();
        if (_interactUI != null)
        {
            _interactUITmp = _interactUI.GetComponentInChildren<TextMeshProUGUI>();
            _icon = _interactUI.GetComponentInChildren<Image>();
        }
    }

    void Update()
    {
        CheckForInteractableObject();
        CheckForExitInteraction();
    }

    private void CheckForInteractableObject()
    {
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 rayDirection = Camera.main.transform.forward; 
        RaycastHit hitInfo;

        bool hitSomething = Physics.Raycast(rayStart, rayDirection, out hitInfo, _rayLength);

        Debug.DrawRay(rayStart, rayDirection, Color.red);

        if (hitSomething && hitInfo.collider.TryGetComponent(out _lookedAtInteractable))
        {
            Debug.DrawRay(rayStart, rayDirection, Color.green);

            _interactUI.SetActive(true);
            _interactUI.transform.position = _lookedAtInteractable.GetInteractableUIPosition();

            if (_playerInputHandler.InteractTriggered)
            {
                _interactableStored = _lookedAtInteractable;
                _interactableStored.OnInteractStart();
            }
        }
        else
        {
            _lookedAtInteractable = null;
            _interactUI.SetActive(false);
        }
    }

    private void CheckForExitInteraction()
    {
        if (_interactableStored == null) return;

        if (_playerInputHandler.ExitInteractionTriggered)
        {
            _interactableStored.OnInteractStop();
            _interactableStored = null;
        }
    }
}
