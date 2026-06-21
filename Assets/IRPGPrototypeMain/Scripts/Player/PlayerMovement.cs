using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _enableJump = false;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _turnSmoothTime = 0.1f;

    [Header("Camera Settings")]
    [SerializeField] private Transform _mainCameraTransform;

    private Animator _playerModelAnimator;
    private Rigidbody _rigidbody;
    private PlayerInputHandler _playerInputHandler;
    private bool _isPlayerFrozen = false;
    private float _turnSmoothVelocity;

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (_mainCameraTransform == null && Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }

        _playerModelAnimator = GetComponentInChildren<Animator>();
        if (_playerModelAnimator != null) Debug.Log("Captured Model Animator");
        else Debug.Log("There is no Animator attached to this Model");
    }

    void Update()
    {   
        if (_enableJump) Jump();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.purple);
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        if (_isPlayerFrozen) return;

        _playerModelAnimator.SetBool("isMoving", false);
        Vector3 inputDirection = new Vector3(_playerInputHandler.MoveInput.x, 0f, _playerInputHandler.MoveInput.y).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            _playerModelAnimator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            _rigidbody.MoveRotation(Quaternion.Euler(0f, angle, 0f));

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

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
