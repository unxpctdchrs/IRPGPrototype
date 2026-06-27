using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private Transform _playerRightHand;
    [SerializeField] private bool _enableAttack = false;

    [Header("Hit Feedback")]
    [SerializeField] private GameObject _hitVFXPrefab;
    [SerializeField] private Vector3 _vfxOffset = new Vector3(0, 1f, 0);
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _transitionDelay = 0.4f;
    
    private Transform _meleeModel;
    private MeleeWeaponHitbox _weaponHitbox;
    private Animator _playerModelAnimator;

    private ISceneService _sceneService;
    private ScenePayload _payload;
    private PartyManager _partyManager;
    private InputManager _inputManager;
    private AudioManager _audioManager;
    private AudioLibrary _audioLibrary;
    private bool _isTransitioningToBattle = false;

    [Inject]
    public void Construct(
        ISceneService sceneService, 
        ScenePayload payload,
        PartyManager partyManager, 
        InputManager inputManager, 
        AudioManager audioManager, 
        AudioLibrary audioLibrary
    ){
    
        _sceneService = sceneService;
        _payload = payload;
        _partyManager = partyManager;
        _inputManager = inputManager;
        _audioManager = audioManager;
        _audioLibrary = audioLibrary;
    }

    void Start()
    {
        _playerModelAnimator = GetComponentInChildren<Animator>();
        if (_impulseSource == null) _impulseSource = GetComponent<CinemachineImpulseSource>();

        if (_playerRightHand != null && _playerRightHand.childCount > 0)
        {
            _meleeModel = _playerRightHand.transform.GetChild(0);
            _weaponHitbox = _meleeModel.GetComponent<MeleeWeaponHitbox>();
            Debug.Log("[PlayerAction] Melee model found");
        }
    }

    void OnEnable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnAttack += CheckOnAttack;
        }
    }

    void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnAttack -= CheckOnAttack;
        }
    }

    private void CheckOnAttack()
    {
        if (!_enableAttack || _isTransitioningToBattle) return;

        if (_weaponHitbox != null)
        {
            _weaponHitbox.ResetHitbox();
        }

        _playerModelAnimator.SetTrigger("isAttacking");
    }

    private void PrepareBattlePayload(EnemyBackpack enemyBackpack)
    {
        if (_payload.BattleEnemies != null && _payload.BattleEnemies.Count > 0) return;

        _payload.Clear();
        _payload.BattleEnemies = enemyBackpack.EncounterProfile.ChildEnemy;
        _payload.CurrentParty = new List<CharacterData>(_partyManager.UnlockedCharacters);
        _payload.DestinationScene = SceneType.BattleScene;
    }

    public void ExecuteMeleeHit()
    {
        if (_isTransitioningToBattle || _weaponHitbox == null) return;

        Collider hitEnemyCollider = _weaponHitbox.PerformHitCheck();
        if (hitEnemyCollider != null && hitEnemyCollider.TryGetComponent(out EnemyBackpack enemyBackpack))
        {
            _isTransitioningToBattle = true; 
            
            PrepareBattlePayload(enemyBackpack);

            hitEnemyCollider.enabled = false;
            if (_impulseSource != null) _impulseSource.GenerateImpulse();
            Debug.Log($"]PlayerAction] hit on: {hitEnemyCollider.gameObject.name}");

            if (_audioManager != null && _audioLibrary.AmbushSFX != null)
            {
                _audioManager.PlaySFXAtPosition(_audioLibrary.AmbushSFX, hitEnemyCollider.transform.position);
            }

            if (_hitVFXPrefab != null)
            {
                Vector3 spawnPos = hitEnemyCollider.transform.position + _vfxOffset;
                GameObject vfx = Instantiate(_hitVFXPrefab, spawnPos, Quaternion.identity);
                Destroy(vfx, 2f); 
            }

            StartCoroutine(TransitionToBattleRoutine());
        }
    }

    public void TriggerEnemyInitiatedBattle(EnemyBackpack enemyBackpack, Vector3 contactPoint)
    {
        if (_isTransitioningToBattle) return; 

        _isTransitioningToBattle = true; 
        PrepareBattlePayload(enemyBackpack);

        if (_impulseSource != null) _impulseSource.GenerateImpulse();

        if (_audioManager != null && _audioLibrary.AmbushSFX != null)
        {
            _audioManager.PlaySFXAtPosition(_audioLibrary.AmbushSFX, contactPoint);;
        }

        if (_hitVFXPrefab != null)
        {
            Destroy(Instantiate(_hitVFXPrefab, contactPoint + _vfxOffset, Quaternion.identity), 2f);
        }

        StartCoroutine(TransitionToBattleRoutine());
    }

    private IEnumerator TransitionToBattleRoutine()
    {
        yield return new WaitForSeconds(_transitionDelay);
        _sceneService.LoadScene(SceneType.BattleScene); 
    }

    public void EnableAttack(bool state)
    {
        _enableAttack = state;
    }

    public void ShowMeleeWeapon(bool state)
    {
        _meleeModel.gameObject.SetActive(state);
    }
}
