using UnityEngine;
using Zenject;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private Transform _playerRightHand;
    [SerializeField] private bool _enableAttack = false;
    private Transform _meleeModel;
    private MeleeWeaponHitbox _weaponHitbox;
    private PlayerInputHandler _playerInputHandler;
    private Animator _playerModelAnimator;

    private ISceneService _sceneService;
    private ScenePayload _payload;

    [Inject]
    public void Construct(ISceneService sceneService, ScenePayload payload)
    {
        _sceneService = sceneService;
        _payload = payload;
    }

    void Start()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerModelAnimator = GetComponentInChildren<Animator>();

        if (_playerRightHand != null && _playerRightHand.childCount > 0)
        {
            _meleeModel = _playerRightHand.transform.GetChild(0);
            _weaponHitbox = _meleeModel.GetComponent<MeleeWeaponHitbox>();
            Debug.Log("Melee model found");
        }
    }

    void Update()
    {
        if (_enableAttack) CheckOnAttack();
    }

    private void CheckOnAttack()
    {
        if (_playerInputHandler.AttackTriggered)
        {
            if (_weaponHitbox != null)
            {
                _weaponHitbox.ResetHitbox();
            }

            _playerModelAnimator.SetTrigger("isAttacking");
        }
    }

    public void ExecuteMeleeHit()
    {
        if (_weaponHitbox != null)
        {
            Collider hitEnemyCollider = _weaponHitbox.PerformHitCheck();
            
            if (hitEnemyCollider != null) 
            {
                Debug.Log($"PlayerAction confirms hit on: {hitEnemyCollider.gameObject.name}");

                if (hitEnemyCollider.TryGetComponent(out EnemyBackpack enemyBackpack))
                {
                    _payload.Clear();
                    _payload.BattleEnemies = enemyBackpack.EncounterProfile.ChildEnemy; 
                    _sceneService.LoadScene(SceneType.BattleScene);
                }
            }
        }
    }
}
