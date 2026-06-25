using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private TargetSelector _targetSelector;
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
        currentBattler.ExecuteTurn(this);
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
        EndCurrentTurn();
    }

    private void ExecutePlayerAttack(IBattler target)
    {
        Debug.Log($"[BattleController] Player is executing an attack on {((MonoBehaviour)target).gameObject.name}!");
        MCBattleController activePlayer = _turnQueue[_currentTurnIndex] as MCBattleController;
        
        if (activePlayer != null)
        {
            activePlayer.PlayAttackAnimation();
        }
        
        // TODO: Trigger the Player's attack animation
        // TODO: Fire your Cinemachine camera shake!
        // TODO: Calculate damage and subtract from the target's HP
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
