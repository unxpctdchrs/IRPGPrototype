using UnityEngine;
using UnityEngine.Playables;
using Fungus;

public class TimelineFungusBridge : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private Flowchart _flowchart;

    public void PlayFungusBlock(string blockName)
    {
        if (_director != null)
        {
            _director.Pause();
        }
        
        if (_flowchart != null)
        {
            _flowchart.ExecuteBlock(blockName);
        }
    }

    public void ResumeTimeline()
    {
        if (_director != null)
        {
            _director.Play();
        }
    }

    public void PauseTimeline()
    {
        if (_director != null)
        {
            _director.Pause();
        }
    }
}