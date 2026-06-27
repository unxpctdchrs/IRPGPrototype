using System.Collections;
using UnityEngine;
using Zenject;

public class SkyboxController : MonoBehaviour
{
    [Header("Skybox Materials")]
    [SerializeField] private Material _normalSkybox; 
    [SerializeField] private Material _hauntedSkybox; 

    [Header("Fog Settings")]
    [SerializeField] private Color _normalFogColor = new Color(0.15f, 0.34f, 0.39f);
    [SerializeField] private Color _hauntedFogColor = new Color(0.51f, 0.19f, 0.20f); 
    
    [Header("Transition Settings")]
    [SerializeField] private float _transitionDuration = 3f;
    private Material _activeSkybox;

    private ISceneService _sceneService;
    [Inject]
    private void Constructor(ISceneService sceneService)
    {
        _sceneService = sceneService;
    }

    void OnEnable()
    {
        _sceneService.OnSceneLoaded += HandleSceneLoaded;
    }

    void OnDisable()
    {
        _sceneService.OnSceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        _activeSkybox = new Material(_normalSkybox);
        RenderSettings.skybox = _activeSkybox;
        RenderSettings.fogColor = _normalFogColor;
    }

    [ContextMenu("TEST: Trigger Normal Environment")]
    public void TriggerNormalEnvironment()
    {
        if (!Application.isPlaying) return;
        
        StopAllCoroutines();
        StartCoroutine(CrossfadeEnvironment(_hauntedSkybox, _normalSkybox, _normalFogColor, targetFogStartDist: 0f));
    }

    [ContextMenu("TEST: Trigger Spooky Environment")]
    public void TriggerSpookyEnvironment()
    {
        if (!Application.isPlaying) return;
        
        StopAllCoroutines();
        StartCoroutine(CrossfadeEnvironment(_normalSkybox, _hauntedSkybox, _hauntedFogColor, targetFogStartDist: -50f));
    }

    private IEnumerator CrossfadeEnvironment(Material startMat, Material endMat, Color targetFogColor, float targetFogStartDist)
    {
        float elapsedTime = 0f;
        
        Color startingFogColor = RenderSettings.fogColor;
        float startingFogStartDist = RenderSettings.fogStartDistance;

        while (elapsedTime < _transitionDuration)
        {
            float t = elapsedTime / _transitionDuration;
            
            _activeSkybox.SetColor("_SkyGradientTop", Color.Lerp(startMat.GetColor("_SkyGradientTop"), endMat.GetColor("_SkyGradientTop"), t));
            _activeSkybox.SetColor("_SkyGradientBottom", Color.Lerp(startMat.GetColor("_SkyGradientBottom"), endMat.GetColor("_SkyGradientBottom"), t));
            _activeSkybox.SetColor("_HorizonLineColor", Color.Lerp(startMat.GetColor("_HorizonLineColor"), endMat.GetColor("_HorizonLineColor"), t));
            _activeSkybox.SetColor("_SunDiscColor", Color.Lerp(startMat.GetColor("_SunDiscColor"), endMat.GetColor("_SunDiscColor"), t));
            _activeSkybox.SetColor("_SunHaloColor", Color.Lerp(startMat.GetColor("_SunHaloColor"), endMat.GetColor("_SunHaloColor"), t));

            _activeSkybox.SetFloat("_SunDiscMultiplier", Mathf.Lerp(startMat.GetFloat("_SunDiscMultiplier"), endMat.GetFloat("_SunDiscMultiplier"), t));
            _activeSkybox.SetFloat("_SunDiscExponent", Mathf.Lerp(startMat.GetFloat("_SunDiscExponent"), endMat.GetFloat("_SunDiscExponent"), t));
            _activeSkybox.SetFloat("_SunHaloExponent", Mathf.Lerp(startMat.GetFloat("_SunHaloExponent"), endMat.GetFloat("_SunHaloExponent"), t));
            _activeSkybox.SetFloat("_SunHaloContribution", Mathf.Lerp(startMat.GetFloat("_SunHaloContribution"), endMat.GetFloat("_SunHaloContribution"), t));
            _activeSkybox.SetFloat("_HorizonLineExponent", Mathf.Lerp(startMat.GetFloat("_HorizonLineExponent"), endMat.GetFloat("_HorizonLineExponent"), t));
            _activeSkybox.SetFloat("_HorizonLineContribution", Mathf.Lerp(startMat.GetFloat("_HorizonLineContribution"), endMat.GetFloat("_HorizonLineContribution"), t));
            _activeSkybox.SetFloat("_SkyGradientExponent", Mathf.Lerp(startMat.GetFloat("_SkyGradientExponent"), endMat.GetFloat("_SkyGradientExponent"), t));

            RenderSettings.fogColor = Color.Lerp(startingFogColor, targetFogColor, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(startingFogStartDist, targetFogStartDist, t);

            RenderSettings.skybox = _activeSkybox;
            DynamicGI.UpdateEnvironment(); 
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to finals
        _activeSkybox.SetColor("_SkyGradientTop", endMat.GetColor("_SkyGradientTop"));
        _activeSkybox.SetColor("_SkyGradientBottom", endMat.GetColor("_SkyGradientBottom"));
        _activeSkybox.SetColor("_HorizonLineColor", endMat.GetColor("_HorizonLineColor"));
        _activeSkybox.SetColor("_SunDiscColor", endMat.GetColor("_SunDiscColor"));
        _activeSkybox.SetColor("_SunHaloColor", endMat.GetColor("_SunHaloColor"));
        
        _activeSkybox.SetFloat("_SunDiscMultiplier", endMat.GetFloat("_SunDiscMultiplier"));
        _activeSkybox.SetFloat("_SunDiscExponent", endMat.GetFloat("_SunDiscExponent"));
        _activeSkybox.SetFloat("_SunHaloExponent", endMat.GetFloat("_SunHaloExponent"));
        _activeSkybox.SetFloat("_SunHaloContribution", endMat.GetFloat("_SunHaloContribution"));
        _activeSkybox.SetFloat("_HorizonLineExponent", endMat.GetFloat("_HorizonLineExponent"));
        _activeSkybox.SetFloat("_HorizonLineContribution", endMat.GetFloat("_HorizonLineContribution"));
        _activeSkybox.SetFloat("_SkyGradientExponent", endMat.GetFloat("_SkyGradientExponent"));

        RenderSettings.fogColor = targetFogColor;
        RenderSettings.fogStartDistance = targetFogStartDist;


        RenderSettings.skybox = _activeSkybox;
        DynamicGI.UpdateEnvironment();
    }

    private void HandleSceneLoaded(SceneType loadedScene)
    {
        switch (loadedScene)
        {
            case SceneType.DebugScene:
                break;
            case SceneType.MainMenuScene:
                break;
            case SceneType.ShedScene:
                RenderSettings.fogStartDistance = 0f;
                break;
            case SceneType.WorldScene:
                TriggerSpookyEnvironment();
                break;
            case SceneType.TransitionScene:
                break;
            case SceneType.BattleScene:
                TriggerSpookyEnvironment();
                break;
            default:
                TriggerNormalEnvironment();
                break;
        }
    }
}