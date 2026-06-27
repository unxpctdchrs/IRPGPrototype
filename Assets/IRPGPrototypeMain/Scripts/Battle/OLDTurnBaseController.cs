using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OLDTurnBaseController : MonoBehaviour
{
    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private TargetSelector _targetSelector;
    [SerializeField] private float _turnDelay = 0.5f;
    private BattleSetup _battleSetup;
    private List<IBattler> _turnQueue = new List<IBattler>();
    private int _currentTurnIndex = 0;

    void Start()
    {
        _battleSetup = GetComponent<BattleSetup>();
        _targetSelector = GetComponent<TargetSelector>();

        BuildTurnQueue();
        
        StartNextTurn();
    }

    private void OnEnable()
    {
        if (_targetSelector != null)
        {
            _targetSelector.OnTargetConfirmed += ExecutePlayerAttack;
        }
    }

    private void OnDisable()
    {
        if (_targetSelector != null)
        {
            _targetSelector.OnTargetConfirmed -= ExecutePlayerAttack;
        }
    }

    private void StartNextTurn()
    {
        if (_turnQueue.Count == 0) return;
        IBattler currentBattler = _turnQueue[_currentTurnIndex];
        // currentBattler.ExecuteTurn(this);
    }

    public void EndCurrentTurn()
    {
        _currentTurnIndex++;

        if (_currentTurnIndex >= _turnQueue.Count)
        {
            _currentTurnIndex = 0;
            BuildTurnQueue();
        }

        StartNextTurn();
    }

    public void ReportTurnFinished()
    {
        StartCoroutine(TurnBreatherRoutine());
    }

    private IEnumerator TurnBreatherRoutine()
    {
        yield return new WaitForSeconds(_turnDelay);        
        EndCurrentTurn();
    }

    private void ExecutePlayerAttack(IBattler target)
    {
        Debug.Log($"[BattleController] Player is executing an attack on {((MonoBehaviour)target).gameObject.name}!");
        IBattler currentBattler = _turnQueue[_currentTurnIndex];
        
        if (currentBattler is IPartyMember activePlayer)
        {
            activePlayer.PlayAttackAnimation(target);
        }
        else
        {
            Debug.LogError("Tried to execute a player attack, but the current turn does not belong to a Party Member");
        }
    }

    public IBattler GetRandomPlayerTarget()
    {
        List<IBattler> activeParty = _battleSetup.GetActiveParty();
        
        if (activeParty.Count == 0) return null;

        int randomIndex = Random.Range(0, activeParty.Count);
        return activeParty[randomIndex];
    }

    private void BuildTurnQueue()
    {
        _turnQueue.Clear();
        _turnQueue.AddRange(_battleSetup.GetActiveParty());

        List<IBattler> enemies = _battleSetup.GetActiveEnemies();
        enemies = enemies.OrderBy(x => Random.value).ToList();
        _turnQueue.AddRange(enemies);
    }
}
