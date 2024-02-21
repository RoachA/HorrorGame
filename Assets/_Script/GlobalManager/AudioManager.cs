using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SfxData[] _sfxList;

    private Dictionary<SfxType, EventInstance> _sfxMap;

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
}