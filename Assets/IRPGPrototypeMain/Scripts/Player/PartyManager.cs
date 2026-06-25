using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("Default Starting Team")]
    [SerializeField] private List<CharacterData> _startingCharacters;
    public List<CharacterData> UnlockedCharacters { get; private set; } = new List<CharacterData>();

    private void Awake()
    {
        if (_startingCharacters != null)
        {
            UnlockedCharacters = new List<CharacterData>(_startingCharacters);
        }
    }

    public void UnlockNewCharacter(CharacterData newCharacter)
    {
        if (newCharacter == null) return;

        if (!UnlockedCharacters.Contains(newCharacter))
        {
            UnlockedCharacters.Add(newCharacter);
            Debug.Log($"[PartyManager] {newCharacter.CharacterName} has joined the party!");
        }
    }
}