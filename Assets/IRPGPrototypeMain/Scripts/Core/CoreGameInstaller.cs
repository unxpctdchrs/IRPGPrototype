using UnityEngine;
using Zenject;

public class CoreGameInstaller : MonoInstaller
{
    [SerializeField] private CoroutineRunner _coroutineRunner;
    [SerializeField] private GameObject _audioManager;
    [SerializeField] private AudioLibrary _mainAudioLibrary;
    [SerializeField] private PartyManager _partyManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private SkyboxController _skyboxController;

    public override void InstallBindings()
    {
        Container.Bind<ISceneService>().To<SceneService>().AsSingle();
        Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();
        Container.Bind<ScenePayload>().AsSingle();
        Container.BindInstance(_mainAudioLibrary).AsSingle();

        Container.BindInterfacesAndSelfTo<AudioManager>()
            .FromComponentInNewPrefab(_audioManager)
            .AsSingle()
            .NonLazy();

        Container.Bind<PartyManager>()
            .FromComponentInNewPrefab(_partyManager)
            .AsSingle()
            .NonLazy();

        Container.Bind<InputManager>()
            .FromComponentInNewPrefab(_inputManager)
            .AsSingle()
            .NonLazy();

        Container.Bind<InventoryManager>()
            .FromComponentInNewPrefab(_inventoryManager)
            .AsSingle()
            .NonLazy();

        Container.Bind<SkyboxController>()
            .FromComponentInNewPrefab(_skyboxController)
            .AsSingle()
            .NonLazy();
    }
}
