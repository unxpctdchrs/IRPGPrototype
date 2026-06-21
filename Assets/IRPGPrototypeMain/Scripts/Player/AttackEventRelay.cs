using UnityEngine;

public class AttackEventRelay : MonoBehaviour
{
    private PlayerAction _playerAction;

    void Start()
    {
        _playerAction = GetComponentInParent<PlayerAction>();
        
        if (_playerAction == null)
        {
            Debug.LogError("AnimationEventRelay could not find PlayerAction on a parent object!");
        }
    }

    public void TriggerMeleeHit()
    {
        if (_playerAction != null) _playerAction.ExecuteMeleeHit();
    }
}