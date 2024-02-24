using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayFmodOneShot : MonoBehaviour
{
    [Inject] private readonly AudioManager _audioManager;
    [SerializeField] private SfxType _type;
    [SerializeField] private string _targetParam;
    [SerializeField] private string _targetParamVal;
    
    private void Start()
    {
        throw new NotImplementedException();
    }

    public void PlayAudio()
    {
        _audioManager.PlayGenericOneShot(_type, gameObject, _targetParam, _targetParamVal);
    }
}
