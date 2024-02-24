using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Game.World.Objects;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SfxData[] _sfxList;

    private Dictionary<SfxType, EventInstance> _sfxMap;
    private Dictionary<SfxType, FMOD.Studio.EventInstance> _sfxInstances = new Dictionary<SfxType, FMOD.Studio.EventInstance>();

    private void Start()
    {
        _sfxMap = new Dictionary<SfxType, EventInstance>();

        foreach (var sfxItem in _sfxList)
        {
            if (_sfxMap.ContainsKey(sfxItem._type) == false)
                _sfxMap.Add(sfxItem._type, RuntimeManager.CreateInstance(sfxItem._event));
        }

        PlayAmbient();
    }

    private void PlayAmbient()
    {
        var sfx = SfxType.Ambient;
        _sfxMap[sfx].start();
    }

    [SerializeField] private float Reverb = 0;

    public void PlayFootstep(FloorSurfaceType floorType, GameObject source, float reverb = 0.1f)
    {
        var sfx = SfxType.Footstep;
        
        if (_sfxMap.ContainsKey(sfx) == false)
        {
            Debug.LogWarning(sfx + " does not exist in the current array.");
            return;
        }

        _sfxMap[sfx].setParameterByNameWithLabel("Type", floorType.ToString());
        _sfxMap[sfx].setParameterByName("Reverb", reverb);
        RuntimeManager.AttachInstanceToGameObject(_sfxMap[sfx], source.transform);
        _sfxMap[sfx].start();
    }

    public void PlayDoorSound(DoorEntity.DoorInteractionType interactionType, GameObject source, float reverb = 0.1f)
    {
        var sfx = SfxType.Door;
        
        if (_sfxMap.ContainsKey(sfx) == false)
        {
            Debug.LogWarning(sfx + " does not exist in the current array.");
            return;
        }

        EventInstance sfxInstance = _sfxMap[sfx];
        
        _sfxMap[sfx].setParameterByNameWithLabel("Door_Action_Type", interactionType.ToString());
        _sfxMap[sfx].setParameterByName("Reverb", reverb);
        RuntimeManager.AttachInstanceToGameObject(_sfxMap[sfx], source.transform);
        _sfxMap[sfx].start();
        _sfxInstances[sfx] = sfxInstance;
    }

    public void StopSound(SfxType type)
    {
        if (_sfxInstances.ContainsKey(type))
        {
            _sfxInstances[type].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _sfxInstances.Remove(type);
        }
    }
    
    public void PlayExclamation(ExclamationType exclamationType, GameObject source, float reverb = 0.5f)
    {
        var sfx = SfxType.Exclamation;
        
        if (_sfxMap.ContainsKey(sfx) == false)
        {
            Debug.LogWarning(sfx + " does not exist in the current array.");
            return;
        }
        
        _sfxMap[sfx].setParameterByNameWithLabel("Exclamation_Type", exclamationType.ToString());
        _sfxMap[sfx].setParameterByName("Reverb", reverb);
        RuntimeManager.AttachInstanceToGameObject(_sfxMap[sfx], source.transform);
        _sfxMap[sfx].start();
    }

    public void PlayGenericOneShot(SfxType type, GameObject source)
    {
        if (_sfxMap.ContainsKey(type) == false)
        {
            Debug.LogWarning(type + " does not exist in the current array.");
            return;
        }
        
        RuntimeManager.AttachInstanceToGameObject(_sfxMap[type], source.transform);
        _sfxMap[type].start();
    }

    public void PlayGenericOneShot(SfxType type, GameObject source, string targetParameter, string targetVal)
    {
        if (_sfxMap.ContainsKey(type) == false)
        {
            Debug.LogWarning(type + " does not exist in the current array.");
            return;
        }
        
        _sfxMap[type].setParameterByNameWithLabel(targetParameter, targetVal);
        RuntimeManager.AttachInstanceToGameObject(_sfxMap[type], source.transform);
        _sfxMap[type].start();
    }
}

[Serializable]
public struct SfxData
{
    public SfxType _type;
    public EventReference _event;
}

public enum SfxType
{
    Footstep = 0,
    Ambient = 1,
    Door = 2,
    Exclamation = 3,
    Flashlight = 4,
}

public enum ExclamationType
{
    Gory = 0,
    DoorSlam = 1,
}