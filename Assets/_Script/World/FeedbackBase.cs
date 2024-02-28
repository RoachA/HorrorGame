using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public enum PlayerEffectType
{
    SwitchFlashLight = 0,
    JitterFlashLight = 1,
}

[Serializable]
public class FeedbackBase : MonoBehaviour
{
    [Inject] protected readonly SignalBus _bus;
    [Inject] protected readonly AudioManager m_audioManager;

    [BoxGroup("Manipulate Objects")]
    [SerializeField] protected bool _manipulateObjects;

    [Sirenix.OdinInspector.ShowIf("_manipulateObjects")]
    [BoxGroup("Manipulate Objects")]
    [SerializeField] private GameObject _targetObject;
    [Sirenix.OdinInspector.ShowIf("_manipulateObjects")]
    [BoxGroup("Manipulate Objects")]
    [SerializeField] private UnityEvent _targetEvent;
           
    [BoxGroup("Affect Player")]
    [SerializeField] protected bool _affectPlayer;
   
    [Sirenix.OdinInspector.ShowIf("_affectPlayer")]
    [BoxGroup("Affect Player")]
    [SerializeField] protected PlayerEffectType _effectType;
   
    [BoxGroup("Audio Cue")]
    [SerializeField] protected bool _audioCue;
    [Sirenix.OdinInspector.ShowIf("_audioCue")]
    [BoxGroup("Audio Cue")]
    [SerializeField] protected GameObject _audioSourceObject;
    [Sirenix.OdinInspector.ShowIf("_audioCue")]
    [BoxGroup("Audio Cue")]
    [SerializeField] protected ExclamationType _cueType;
   
    public virtual void PlayFeedback()
    {
        if (_audioCue) CallForAudioFeedback();
        if (_affectPlayer) CallForEffectOnPlayer();
        if (_manipulateObjects) CallForObjectManipulation();
    }
           
    protected virtual void CallForEffectOnPlayer()
    {
        switch (_effectType)
        {
            case PlayerEffectType.SwitchFlashLight:
                _bus.Fire(new CoreSignals.OnAffectFlashLightSignal(FlashLightAction.Switch));
                break;
            case PlayerEffectType.JitterFlashLight:
                _bus.Fire(new CoreSignals.OnAffectFlashLightSignal(FlashLightAction.TriggerJitter, 4));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected void CallForObjectManipulation()
    {
        if (_targetEvent != null) _targetEvent.Invoke();
    }
           
    [Sirenix.OdinInspector.ShowIf("_audioCue")]
    [BoxGroup("Audio Cue")]
    [Button]
    protected void CreateAudioObject()
    {
        if (_audioSourceObject != null) return;
        var newSource = new GameObject("audio source");
        newSource.transform.position = transform.position;
        newSource.transform.SetParent(transform);
   
        Selection.activeGameObject = _audioSourceObject;
        _audioSourceObject = newSource;
    }
   
    protected void CallForAudioFeedback()
    {
        if (_audioSourceObject == null)
        {
            _audioSourceObject = gameObject;
            Debug.LogWarning(gameObject.name + " has no audio source object, so it uses itself.");
        }
               
        m_audioManager.PlayExclamation(_cueType, _audioSourceObject);
    }
           
    protected void OnDrawGizmosSelected()
    {
        if (_audioSourceObject != null)
        {
            Gizmos.DrawIcon(_audioSourceObject.transform.position + Vector3.up, "audio_gizmo");
            Gizmos.color = Color.green * 0.8f; 
            Gizmos.DrawWireSphere(_audioSourceObject.transform.position + Vector3.up, 3);
        }
    }
}