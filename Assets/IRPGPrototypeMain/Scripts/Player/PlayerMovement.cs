using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _enableJump = false;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    private Vector2 _currentMoveInput;

    [Header("Camera Settings")]
    [SerializeField] private Transform _mainCameraTransform;

    [Header("Intro Cinematic Settings")]
    [SerializeField] private Transform _cinematicTarget;
    [SerializeField] private float _cinematicWalkSpeed = 3f;
    [SerializeField] private bool _isGrounded = false;

    private Animator _playerModelAnimator;
    private Rigidbody _rigidbody;
    private bool _isPlayerFrozen = false;
    private float _turnSmoothVelocity;
    private float _distanceTraveled;
    private float _stepDistanceToPlaySound = 3.0f;

    private AudioManager _audioManager;
    private AudioLibrary _audioLibrary;
    private InputManager _inputManager;

    [Inject]
    public void Construct(AudioManager audioManager, AudioLibrary audioLibrary, InputManager inputManager)
    {
        _audioManager = audioManager;
        _audioLibrary = audioLibrary;
        _inputManager = inputManager;
    }

    void Start()
    {
        // _playerInputHandler = GetComponent<PlayerInputHandler>();
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

    void OnEnable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnMove += UpdateMovementInput;
            _inputManager.OnJump += HandleJump;
        }
    }

    void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnMove -= UpdateMovementInput;
            _inputManager.OnJump -= HandleJump;
        }
    }

    void Update()
    { 
        Debug.DrawRay(transform.position, transform.forward * 2, Color.purple);
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    private void UpdateMovementInput(Vector2 newMoveInput)
    {
        _currentMoveInput = newMoveInput;
    }

    private void MoveAndRotate()
    {
        if (_isPlayerFrozen) return;

        _playerModelAnimator.SetBool("isMoving", false);
        Vector3 inputDirection = new Vector3(_currentMoveInput.x, 0f, _currentMoveInput.y).normalized;

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

    private void HandleJump()
    {
        if (_enableJump && !_isPlayerFrozen && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        float rayLength = 1.5f;
        bool iG = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, rayLength, _groundLayer);
        _isGrounded = iG;
        return iG;
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

    // I think I can decouple this in the future
    public void WalkToCinematicMark()
    {
        StartCoroutine(IntroWalkRoutine());
    }

    private IEnumerator IntroWalkRoutine()
    {
        FreezePlayer(true);
        _playerModelAnimator.SetBool("isMoving", true);

        while (true)
        {
            Vector3 flatTarget = new Vector3(_cinematicTarget.position.x, transform.position.y, _cinematicTarget.position.z);
            
            if (Vector3.Distance(transform.position, flatTarget) <= 0.1f)
            {
                break; 
            }

            Vector3 newPos = Vector3.MoveTowards(transform.position, flatTarget, _cinematicWalkSpeed * Time.deltaTime);
            _rigidbody.MovePosition(newPos);

            Vector3 direction = (flatTarget - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            yield return new WaitForFixedUpdate();
        }
        _playerModelAnimator.SetBool("isMoving", false);
    }

    private void HandleFootsteps()
    {
        bool isWalkingManually = !_isPlayerFrozen && _currentMoveInput.magnitude > 0.1f;
        bool isWalkingCinematically = _isPlayerFrozen && _playerModelAnimator.GetBool("isMoving");

        if (IsGrounded() && (isWalkingManually || isWalkingCinematically))
        {
            float activeSpeed = isWalkingCinematically ? _cinematicWalkSpeed : _walkSpeed;
            
            _distanceTraveled += activeSpeed * Time.deltaTime;

            if (_distanceTraveled >= _stepDistanceToPlaySound)
            {
                if (_audioLibrary != null) 
                {
                    _audioManager.PlaySFXAtPosition(_audioLibrary.StepConcrete, transform.position);
                }
                _distanceTraveled = 0f;
            }
        }
    }
}
