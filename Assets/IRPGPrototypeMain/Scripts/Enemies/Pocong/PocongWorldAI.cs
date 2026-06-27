using System.Collections;
using UnityEngine;

public class PocongWorldAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    private Animator animator;

    [Header("Settings")]
    [SerializeField] private float _chaseDistance = 10f; 
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _timeBetweenTurns = 3f; 
    [SerializeField] private float _aiTickRate = 0.25f; 
    
    private float wanderTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(AILogicTick());
    }

    IEnumerator AILogicTick()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            isChasing = distanceToPlayer <= _chaseDistance;
            yield return new WaitForSeconds(_aiTickRate);
        }
    }

    void Update()
    {
        if (isChasing)
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
        animator.SetBool("isMoving", true);
        Vector3 directionToPlayer = _player.position - transform.position;
        directionToPlayer.y = 0; 
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    void WanderAround()
    {
        animator.SetBool("isMoving", true);
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= _timeBetweenTurns)
        {
            float randomAngle = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
            wanderTimer = 0f; 
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
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