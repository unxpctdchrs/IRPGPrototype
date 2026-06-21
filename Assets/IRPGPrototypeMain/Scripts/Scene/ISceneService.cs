using System;

public enum SceneType 
{ 
    DebugScene,
    MainMenuScene,
    ShedScene, 
    WorldScene,
    BattleScene
}

public interface ISceneService
{
    event Action<SceneType> OnSceneLoaded;
    void LoadScene(SceneType sceneType);
    SceneType CurrentScene { get; }
    void LoadScene(string sceneName);
}
