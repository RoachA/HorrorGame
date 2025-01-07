using System;
using UnityEngine;
using Zenject;

public class FmodAudioSource3d : MonoBehaviour
{
    [Inject] private readonly AudioManager _audioManager;
    [SerializeField] private SfxType _type;
    
    void Start()
    {
        _audioManager.PlayGenericOneShot(_type, gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "audio_gizmo");
    }
}
