using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCommand
{
    public IPartyMember Caster;
    public IBattler Target;
    public SkillData Skill; // null = no skill
}

public class TurnBaseController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private BattleSetup _battleSetup;
    [SerializeField] private BattleUIManager _battleUIManager;

    [Header("Targeting Selectors")]
    [SerializeField] private TargetSelector _characterSelector;
    [SerializeField] private TargetSelector _enemySelector; 

    [Header("Cinematic Cameras")]
    [SerializeField] private GameObject _playerPhaseCamera;
    [SerializeField] private GameObject _enemyPhaseCamera;

    [SerializeField] private float _turnDelay = 0.5f;

    private List<IBattler> _playersYetToAct = new List<IBattler>();
    private List<IBattler> _enemyQueue = new List<IBattler>();
    private Queue<PlayerCommand> _playerCommands = new Queue<PlayerCommand>(); 
    private IBattler _activePlayer;
    private bool _isBreatherRunning = false; 

    private enum BattleState { Selection, PlayerExecution, EnemyExecution }
    private BattleState _currentState;
    private SkillData _pendingSkill;

    private IEnumerator Start() // didn't know unity can do this
    {
        if (_battleSetup == null) _battleSetup = GetComponent<BattleSetup>();
        if (_battleUIManager == null) _battleUIManager = FindAnyObjectByType<BattleUIManager>();

        yield return new WaitUntil(() => _battleSetup.GetActiveParty().Count > 0 && _battleSetup.GetActiveEnemies().Count > 0);

        StartSelectionPhase();
    }

    private void OnEnable()
    {
        if (_characterSelector != null) _characterSelector.OnTargetConfirmed += OnCharacterSelected;
        if (_enemySelector != null) 
        {
            _enemySelector.OnTargetConfirmed += QueuePlayerCommand; 
            _enemySelector.OnSelectionCanceled += OnEnemySelectionCanceled;
        }
    }

    private void OnDisable()
    {
        if (_characterSelector != null) _characterSelector.OnTargetConfirmed -= OnCharacterSelected;
        if (_enemySelector != null) 
        {
            _enemySelector.OnTargetConfirmed -= QueuePlayerCommand; 
            _enemySelector.OnSelectionCanceled -= OnEnemySelectionCanceled;
        }
    }

    // ==========================================
    // PHASE 1: SELECTION
    // ==========================================
    private void StartSelectionPhase()
    {
        _currentState = BattleState.Selection;
        _playersYetToAct.Clear();
        _playerCommands.Clear();
        _playersYetToAct.AddRange(_battleSetup.GetActiveParty());
        PromptNextCharacterSelection();
    }

    private void PromptNextCharacterSelection()
    {
        _battleUIManager.CloseActionMenu();
        _playersYetToAct.RemoveAll(p => !_battleSetup.GetActiveParty().Contains(p)); // Clean dead bodies

        if (_battleSetup.GetActiveEnemies().Count == 0)
        {
            Debug.Log("<color=green>BATTLE WON!</color>");
            return; 
        }

        // If everyone has been assigned a target, stop selecting and start attacking
        if (_playersYetToAct.Count == 0)
        {
            StartPlayerExecutionPhase();
            return;
        }

        _playerPhaseCamera.SetActive(true);
        _enemyPhaseCamera.SetActive(false);
        _characterSelector.StartSelection(_playersYetToAct);
    }

    private void OnCharacterSelected(IBattler chosenCharacter)
    {
        _activePlayer = chosenCharacter;
        _enemyPhaseCamera.SetActive(true);
        chosenCharacter.ExecuteTurn(this); 
    }

    public void StartEnemySelection()
    {
        _playerPhaseCamera.SetActive(false);
        _enemyPhaseCamera.SetActive(true);

        List<IBattler> aliveEnemies = _battleSetup.GetActiveEnemies();
        _enemySelector.StartSelection(aliveEnemies);
    }

    private void QueuePlayerCommand(IBattler target)
    {
        if (_activePlayer == null) return;

        if (_activePlayer is IPartyMember activePartyMember)
        {
            _playerCommands.Enqueue(new PlayerCommand { Caster = activePartyMember, Target = target, Skill = _pendingSkill }); // Store the attack in the queue instead before executing it
            _pendingSkill = null; // clear the pending skill so it wouldn't get carried to the next character   
            _playersYetToAct.Remove(_activePlayer); // if it has been queued, remove it, so it doesnt get picked again
            PromptNextCharacterSelection(); // if there is any characters left, it will go into this
        }
    }

    // ==========================================
    // PHASE 2: PLAYER EXECUTION
    // ==========================================
    private void StartPlayerExecutionPhase()
    {
        _currentState = BattleState.PlayerExecution;
        _playerPhaseCamera.SetActive(false);
        ExecuteNextPlayerCommand();
    }

    private void ExecuteNextPlayerCommand()
    {
        // If the player queue is empty, hand the turn over to the enemies
        if (_playerCommands.Count == 0)
        {
            StartEnemyPhase();
            return;
        }

        PlayerCommand nextCommand = _playerCommands.Dequeue();

        // did this character die while waiting in the queue?
        if (!_battleSetup.GetActiveParty().Contains((IBattler)nextCommand.Caster))
        {
            ReportTurnFinished(); // skip their turn
            return;
        }

        // execute the command
        if (nextCommand.Skill == null)
        {
            nextCommand.Caster.PlayAttackAnimation(nextCommand.Target);
        }
        else
        {
            nextCommand.Caster.PlaySkillAnimation(nextCommand.Target, nextCommand.Skill);
        }
    }

    // ==========================================
    // PHASE 3: ENEMY EXECUTION
    // ==========================================
    private void StartEnemyPhase()
    {
        _currentState = BattleState.EnemyExecution;
        _enemyQueue.Clear();
        _enemyQueue.AddRange(_battleSetup.GetActiveEnemies().OrderBy(x => Random.value));
        
        ExecuteNextEnemyTurn();
    }

    private void ExecuteNextEnemyTurn()
    {
        _enemyQueue.RemoveAll(e => !_battleSetup.GetActiveEnemies().Contains(e)); // clean dead bodies

        if (_enemyQueue.Count == 0)
        {
            StartSelectionPhase();
            return;
        }

        if (_battleSetup.GetActiveParty().Count == 0)
        {
            Debug.Log("<color=red>BATTLE LOST!</color>");
            return; 
        }

        IBattler nextEnemy = _enemyQueue[0];
        _enemyQueue.RemoveAt(0);

        nextEnemy.ExecuteTurn(this);
    }

    // ==========================================
    // SKILLS
    // ==========================================
    public void OnSkillChosen(SkillData skill)
    {
        _pendingSkill = skill;
        StartSkillTargeting(skill);
    }

    private void StartSkillTargeting(SkillData skill)
    {
        switch (skill.TargetRequirement)
        {
            case TargetType.Enemy:
                _playerPhaseCamera.SetActive(false);
                _enemyPhaseCamera.SetActive(true);
                _enemySelector.StartSelection(_battleSetup.GetActiveEnemies());
                break;

            case TargetType.Ally:
                _enemyPhaseCamera.SetActive(false);
                _playerPhaseCamera.SetActive(true);
                _enemySelector.StartSelection(_battleSetup.GetActiveParty());
                break;

            case TargetType.Any:
                _playerPhaseCamera.SetActive(false);
                _enemyPhaseCamera.SetActive(true);
                List<IBattler> allTargets = new List<IBattler>();
                allTargets.AddRange(_battleSetup.GetActiveParty());
                allTargets.AddRange(_battleSetup.GetActiveEnemies());
                _enemySelector.StartSelection(allTargets);
                break;
        }
    }    

    // ==========================================
    // UTILITIES
    // ==========================================
    public void ReportTurnFinished()
    {
        if (!_isBreatherRunning)
        {
            StartCoroutine(TurnBreatherRoutine());
        }
    }

    private IEnumerator TurnBreatherRoutine()
    {
        _isBreatherRunning = true; 
        yield return new WaitForSeconds(_turnDelay);  
        _isBreatherRunning = false;
        
        if (_currentState == BattleState.PlayerExecution)
        {
            ExecuteNextPlayerCommand();
        }
        else if (_currentState == BattleState.EnemyExecution)
        {
            ExecuteNextEnemyTurn();
        }
    }

    public IBattler GetRandomPlayerTarget()
    {
        List<IBattler> activeParty = _battleSetup.GetActiveParty();
        if (activeParty.Count == 0) return null;
        return activeParty[Random.Range(0, activeParty.Count)];
    }

    private void OnEnemySelectionCanceled()
    {
        _enemyPhaseCamera.SetActive(false);
        _playerPhaseCamera.SetActive(true);   
        if (_activePlayer != null)
        {
            _activePlayer.ExecuteTurn(this); 
        }
    }
}