using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private Transform _playerRightHand;
    [SerializeField] private bool _enableAttack = false;
    private Transform _meleeModel;
    private MeleeWeaponHitbox _weaponHitbox;
    private Animator _playerModelAnimator;

    private ISceneService _sceneService;
    private ScenePayload _payload;
    private PartyManager _partyManager;
    private InputManager _inputManager;

    [Inject]
    public void Construct(ISceneService sceneService, ScenePayload payload, PartyManager partyManager, InputManager inputManager)
    {
        _sceneService = sceneService;
        _payload = payload;
        _partyManager = partyManager;
        _inputManager = inputManager;
    }

    void Start()
    {
        _playerModelAnimator = GetComponentInChildren<Animator>();

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
        if (!_enableAttack) return;

        if (_weaponHitbox != null)
        {
            _weaponHitbox.ResetHitbox();
        }

        _playerModelAnimator.SetTrigger("isAttacking");
    }

    public void ExecuteMeleeHit()
    {
        if (_weaponHitbox != null)
        {
            Collider hitEnemyCollider = _weaponHitbox.PerformHitCheck();
            
            if (hitEnemyCollider != null) 
            {
                Debug.Log($"]PlayerAction] hit on: {hitEnemyCollider.gameObject.name}");

                if (hitEnemyCollider.TryGetComponent(out EnemyBackpack enemyBackpack))
                {
                    _payload.Clear();
                    _payload.BattleEnemies = enemyBackpack.EncounterProfile.ChildEnemy;
                    _payload.CurrentParty = new List<CharacterData>(_partyManager.UnlockedCharacters);
                    _sceneService.LoadScene(SceneType.BattleScene);
                }
            }
        }
    }
}
