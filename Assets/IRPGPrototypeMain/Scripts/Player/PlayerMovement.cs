using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _turnSmoothTime = 0.1f;

    [Header("Camera Settings")]
    [Tooltip("The main camera so the player moves relative to where the camera is looking.")]
    [SerializeField] private Transform _mainCameraTransform;

    private Rigidbody _rigidbody;
    private PlayerInputHandler _playerInputHandler;
    private bool _isPlayerFrozen = false;
    private float _turnSmoothVelocity;

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // Auto-assign the main camera if one isn't dragged into the inspector
        if (_mainCameraTransform == null && Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {   
        Jump();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.purple);
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        if (_isPlayerFrozen) return;

        // Capture raw input
        Vector3 inputDirection = new Vector3(_playerInputHandler.MoveInput.x, 0f, _playerInputHandler.MoveInput.y).normalized;

        // Only move and rotate if there is input
        if (inputDirection.magnitude >= 0.1f)
        {
            // Calculate the target angle based on input and camera rotation
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCameraTransform.eulerAngles.y;

            // Smoothly rotate the character model to face the movement direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            _rigidbody.MoveRotation(Quaternion.Euler(0f, angle, 0f));

            // Calculate the actual movement direction relative to the camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Apply movement
            _rigidbody.MovePosition(_rigidbody.position + moveDir.normalized * _walkSpeed * Time.fixedDeltaTime);
        }
    }

    private void Jump()
    {
        if (_playerInputHandler.JumpTriggered && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        float rayLength = 1.5f;
        // Added a slight upward offset so the raycast doesn't start exactly at floor level
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, rayLength, _groundLayer);
    }

    public void MovePlayer(Vector3 position)
    {
        _rigidbody.MovePosition(position);
    }

    public void FreezePlayer(bool isFrozen)
    {
        _isPlayerFrozen = isFrozen;
    }

    public bool IsPlayerFrozen() => _isPlayerFrozen;
}
