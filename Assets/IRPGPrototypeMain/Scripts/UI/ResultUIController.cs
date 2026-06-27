using UnityEngine;
using System.Collections.Generic;
using Zenject;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultUIController : MonoBehaviour
{
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private ResultContentUI _rowPrefab;
    [SerializeField] private Button _nextButton;

    private BattleRewardTracker _rewardTracker;
    private ISceneService _sceneService;

    [Inject]
    public void Construct(BattleRewardTracker rewardTracker, ISceneService sceneService)
    {
        _rewardTracker = rewardTracker;
        _sceneService = sceneService;
    }

    void Start()
    {
        if(_nextButton != null) _nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void ShowResults()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (_nextButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(_nextButton.gameObject);
        }

        foreach (Transform child in _contentContainer)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> groupedRewards = new Dictionary<string, int>();
        
        foreach (ItemData item in _rewardTracker.DroppedItems)
        {
            string name = item.ItemName; 

            if (groupedRewards.ContainsKey(name))
            {
                groupedRewards[name]++;
            }
            else
            {
                groupedRewards[name] = 1;
            }
        }

        foreach (var reward in groupedRewards)
        {
            ResultContentUI row = Instantiate(_rowPrefab, _contentContainer);
            row.Setup(reward.Key, reward.Value);
        }
    }

    private void OnNextButtonClicked()
    {
        if (_rewardTracker != null)
        {
            _rewardTracker.ClearRewards();
        }

        if (_sceneService != null)
        {
            _sceneService.LoadScene(SceneType.WorldScene);
        }
    }
}