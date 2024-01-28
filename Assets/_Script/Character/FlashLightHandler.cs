using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlashLightHandler : MonoBehaviour
{
    [SerializeField] private Vector2 _offsetMultiplier = new Vector2(0.5f, 2f);
    [SerializeField] private float _offsettingDuration = .5f;
    private Light m_light;
    private bool m_isObscured;
    private bool m_isOn = true;
    private const float m_litIntensity = 9;
    private Sequence _seq;
    private Sequence _jitterSeq;
    private void Start()
    {
        m_light = GetComponentInChildren<Light>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_isObscured) return;
        if (other.CompareTag("Wall") == false) return;
        m_isObscured = true;
        
        _seq?.Kill();
        _seq = DOTween.Sequence();

        _seq.Insert(0.1f, m_light.transform.DOLocalMoveX(_offsetMultiplier.y, _offsettingDuration));
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (m_isObscured == false) return;
        if (other.CompareTag("Wall") == false) return;
        m_isObscured = false;
        
        _seq?.Kill();
        _seq = DOTween.Sequence();

        _seq.Insert(.25f, m_light.transform.DOLocalMoveX(_offsetMultiplier.x, _offsettingDuration));
    }
    
    public void SwitchFlashlight()
    {
        m_isOn = !m_isOn;
        
        m_light.intensity = m_isOn ? m_litIntensity : 0;
    }

    [Sirenix.OdinInspector.Button]
    public void StartJitter()
    {
        _jitterSeq?.Kill(true);
        _jitterSeq = DOTween.Sequence().SetLoops(-1);

        var s = 0;

        _jitterSeq.Append(DOTween.To(() => s, x => s = x, 1, .1f)
            .OnStepComplete(() =>
            {
                float randomIntensity = m_isOn ? UnityEngine.Random.Range(0f, 12f) : 0;
                m_light.intensity = randomIntensity;
            }));
    }

    [Sirenix.OdinInspector.Button]
    public void StopJitter()
    {
       _jitterSeq?.Kill(true);
       m_light.intensity = m_litIntensity;
    }
}
