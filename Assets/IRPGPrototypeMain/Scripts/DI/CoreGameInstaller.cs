using Zenject;
using UnityEngine;

public class CoreGameInstaller : MonoInstaller
{
    [SerializeField] private CoroutineRunner _coroutineRunner;
    [SerializeField] private GameObject _audioManagerPrefab;
    [SerializeField] private AudioLibrary _mainAudioLibrary;
    public override void InstallBindings()
    {
        Container.Bind<ISceneService>().To<SceneService>().AsSingle();
        Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
        Container.Bind<ScenePayload>().AsSingle();
        Container.BindInterfacesAndSelfTo<AudioManager>()
            .FromComponentInNewPrefab(_audioManagerPrefab)
            .AsSingle()
            .NonLazy();
        Container.BindInstance(_mainAudioLibrary).AsSingle();
    }
}
