using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonFeedback : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visuals")]
    [SerializeField] private GameObject _selectionBorder;

    private void Start()
    {
        HideBorder();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowBorder();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideBorder();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowBorder();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideBorder();
    }

    private void ShowBorder()
    {
        if (_selectionBorder != null) _selectionBorder.SetActive(true);
    }

    private void HideBorder()
    {
        if (_selectionBorder != null) _selectionBorder.SetActive(false);
    }
}