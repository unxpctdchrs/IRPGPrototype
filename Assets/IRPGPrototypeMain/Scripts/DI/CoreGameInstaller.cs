using Zenject;
using UnityEngine;

public class CoreGameInstaller : MonoInstaller
{
    [SerializeField] private CoroutineRunner _coroutineRunner;
    public override void InstallBindings()
    {
        Container.Bind<ISceneService>().To<SceneService>().AsSingle();
        Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
        Container.Bind<ScenePayload>().AsSingle();
    }
}
