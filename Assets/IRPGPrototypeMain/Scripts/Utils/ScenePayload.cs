using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType 
{ 
    Normal, 
    Spooky 
}

public class ScenePayload
{
    public EnvironmentType TargetEnvironment { get; set; }    
    public SceneType DestinationScene { get; set; }
    public List<GameObject> BattleEnemies { get; set; } 

    public void Clear()
    {
        TargetEnvironment = EnvironmentType.Normal;
        DestinationScene = SceneType.MainMenuScene; 
        
        if (BattleEnemies != null)
        {
            BattleEnemies.Clear();
        }
    }
}