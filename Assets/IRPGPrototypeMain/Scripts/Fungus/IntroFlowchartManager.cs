using UnityEngine;
using Fungus;
using Zenject;
using UnityEngine.Playables;

public class IntroFlowchartManager : MonoBehaviour
{
    [SerializeField] private Flowchart _introFlowchart;
    [SerializeField] private PlayableDirector _introTimeline;
    [SerializeField] private Transform _bobLastPosition;
    [SerializeField] private Transform _bob;
    
    private ScenePayload _payload;

    [Inject]
    public void Construct(ScenePayload payload)
    {
        _payload = payload;
    }

    private void Awake()
    {
        if (_payload.HasPlayedShedIntro)
        {
            Debug.Log("[IntroManager] Intro already played. Skipping Flowchart and Timeline");
            
            if (_introFlowchart != null) 
                _introFlowchart.gameObject.SetActive(false);
            if (_introTimeline != null) 
                _introTimeline.gameObject.SetActive(false);
            
            if(_bob != null && _bobLastPosition != null)
            {
                _bob.SetParent(_bobLastPosition);
                _bob.localPosition = Vector3.zero;
            }
        }
    }
}