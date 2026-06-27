using UnityEngine;
using Fungus;
using Zenject;

public class OldMan : MonoBehaviour, IInteractable
{
    [Header("Required Items")]
    [SerializeField] private ItemData _jantungPocong;
    [SerializeField] private ItemData _jantungKuntilanak;

    [Header("Dialogue Messages")]
    [SerializeField] private string _missingItemsMessage = "OldManNeedItems";
    [SerializeField] private string _questCompleteMessage = "OldManQuestComplete";

    [Header("UI")]
    [SerializeField] private Vector3 _uiOffset = new Vector3(-0.5f, 1.7f, 0);

    private InventoryManager _inventoryManager;
    private ISceneService _sceneService;

    [Inject]
    public void Construct(InventoryManager inventoryManager, ISceneService sceneService)
    {
        _inventoryManager = inventoryManager;
        _sceneService = sceneService;
    }

    public void OnInteractStart()
    {
        bool hasAllItems = _inventoryManager.HasItem(_jantungPocong) && _inventoryManager.HasItem(_jantungKuntilanak);

        if (hasAllItems)
        {
            _inventoryManager.RemoveItem(_jantungPocong);
            _inventoryManager.RemoveItem(_jantungKuntilanak);
            Flowchart.BroadcastFungusMessage(_questCompleteMessage);
        }
        else
        {
            Flowchart.BroadcastFungusMessage(_missingItemsMessage);
        }
    }

    public void OnInteractStop() { }

    public string GetInteractText() => "Talk";

    public Vector3 GetInteractableUIPosition() => transform.position + _uiOffset;

    public void LoadThankYouScene()
    {
        _sceneService.LoadScene(SceneType.ThankYou);
    }
}