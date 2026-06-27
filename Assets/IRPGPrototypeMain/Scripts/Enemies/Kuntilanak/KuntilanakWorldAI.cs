using System.Collections;
using UnityEngine;

public class KuntilanakWorldAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _kuntilanakModel;
    private Animator _animator;

    [Header("Settings")]
    [SerializeField] private float _chaseDistance = 10.0f; 
    [SerializeField] private float _moveSpeed = 3.0f;
    [SerializeField] private float _rotationSpeed = 5.0f;
    [SerializeField] private float _timeBetweenTurns = 3.0f; 
    [SerializeField] private float _aiTickRate = 0.25f; 
    private float _orientationWhenWandering = 0.0f;
    private float _orientationWhenChasing = -25.0f;
    private float _tiltSpeed = 5f;
    private float _currentTilt = 0.0f;
    
    private float _wanderTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool _isChasing = false;

    private HoverObject _hover;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (_kuntilanakModel == null) Debug.LogWarning("Kunti Model is not assigned yet, pose will remain on walk anim");
        _hover = GetComponent<HoverObject>();

        StartCoroutine(AILogicTick());
    }

    IEnumerator AILogicTick()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            _isChasing = distanceToPlayer <= _chaseDistance;
            yield return new WaitForSeconds(_aiTickRate);
        }
    }

    void Update()
    {
        if (_isChasing)
        {
            ChasePlayer();
        }
        else
        {
            WanderAround();
        }
    }

    void ChasePlayer()
    {
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isChasing", true);
        TiltModel(_orientationWhenChasing);
        _hover.EnableHover(true);

        Vector3 directionToPlayer = _player.position - transform.position;
        directionToPlayer.y = 0; 
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    void WanderAround()
    {
        _animator.SetBool("isChasing", false);
        _animator.SetBool("isWalking", true);
        TiltModel(_orientationWhenWandering);
        _hover.EnableHover(false);

        _wanderTimer += Time.deltaTime;
        if (_wanderTimer >= _timeBetweenTurns)
        {
            float randomAngle = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
            _wanderTimer = 0f; 
        }
        transform.position += transform.forward * (_moveSpeed * 0.5f) * Time.deltaTime;
    }

    private void TiltModel(float targetAngle)
    {
        _currentTilt = Mathf.Lerp(_currentTilt, targetAngle, _tiltSpeed * Time.deltaTime);
        _kuntilanakModel.transform.localEulerAngles = new Vector3(_currentTilt, 0, 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, _chaseDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerAction playerAction))
            {
                playerAction.TriggerEnemyInitiatedBattle(GetComponent<EnemyBackpack>(), collision.GetContact(0).point);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerAction playerAction))
            {
                playerAction.TriggerEnemyInitiatedBattle(GetComponent<EnemyBackpack>(), other.ClosestPoint(transform.position));
            }
        }
    }
}