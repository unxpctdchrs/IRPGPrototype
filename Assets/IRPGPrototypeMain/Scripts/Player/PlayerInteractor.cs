using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerInteractor : MonoBehaviour
{
    public bool IsInteracting => _interactableStored != null;
    [SerializeField] private bool _enableInteraction = false;

    [Header("UI")]
    [SerializeField] private GameObject _interactUI;
    
    [Header("Raycast Settings")]
    [SerializeField] private float _sphereRadius = 0.5f;

    private IInteractable _interactableStored;
    IInteractable _lookedAtInteractable;
    private TextMeshProUGUI _interactUITmp;
    private Image _icon;
    private Collider[] _resultBuffer = new Collider[5];

    private InputManager _inputManager;

    [Inject]
    public void Construct(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    void Start()
    {
        if (_interactUI != null)
        {
            _interactUITmp = _interactUI.GetComponentInChildren<TextMeshProUGUI>();
            _icon = _interactUI.GetComponentInChildren<Image>();
        }
    }

    private void OnEnable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnInteract += HandleInteractStart;
            _inputManager.OnCancel += HandleInteractExit;
        }
    }

    private void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnInteract -= HandleInteractStart;
            _inputManager.OnCancel -= HandleInteractExit;
        }
    }

    void Update()
    {
        if (!IsInteracting && _enableInteraction)
        {
            CheckForInteractableObject();
        }
    }

    private void CheckForInteractableObject()
    {
        _interactUI.SetActive(false);
        Vector3 playerCenter = transform.position;

        int numFound = Physics.OverlapSphereNonAlloc(playerCenter, _sphereRadius, _resultBuffer);

        IInteractable bestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < numFound; i++)
        {
            Collider col = _resultBuffer[i];
            
            if (col.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(playerCenter, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = interactable;
                }
            }
        }
        
        _lookedAtInteractable = bestTarget;

        if (_lookedAtInteractable != null)
        {
            _interactUI.SetActive(true);
            _interactUI.transform.position = _lookedAtInteractable.GetInteractableUIPosition();
            
            if (_interactUITmp != null) 
            {
                _interactUITmp.text = _lookedAtInteractable.GetInteractText();
            }
        }
        else
        {
            _interactUI.SetActive(false);
        }
    }

    private void HandleInteractStart()
    {
        if (!IsInteracting && _lookedAtInteractable != null)
        {
            _interactableStored = _lookedAtInteractable;
            _interactableStored.OnInteractStart();
        }
    }

    private void HandleInteractExit()
    {
        if (IsInteracting)
        {
            _interactableStored.OnInteractStop();
            _interactableStored = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        bool foundSomething = _lookedAtInteractable != null;
        Gizmos.color = foundSomething ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _sphereRadius);
    }

    public void EnableInteraction(bool state)
    {
        _enableInteraction = state;
        _interactUI.SetActive(state);
    }
}