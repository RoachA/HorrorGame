using System;
using DG.Tweening;
using UnityEngine;

public class FlashLightHandler : MonoBehaviour
{
    [SerializeField] private Vector2 _offsetMultiplier = new Vector2(0.5f, 2f);
    [SerializeField] private float _offsettingDuration = .5f;
    [SerializeField] private FlashLightParam _defaultParams;
    [SerializeField] private FlashLightParam _focusedParams;
    [SerializeField] private LightSourceMesher _lightSourceMesh;
    private Light m_light;
    private bool m_isObscured;
    private bool m_isOn = true;
    private const float m_defaultIntensity = 9;
    private Sequence _seq;
    private Sequence _focusSeq;
    private Sequence _jitterSeq;
    private bool _hasMesh = false;

    [Serializable]
    private struct FlashLightParam
    {
        public Vector2 ConeRanges;
        public float Intensity;
        public float Range;
    }
    
    private void Start()
    {
        m_light = GetComponentInChildren<Light>();
        SetLightParam(false);
        _hasMesh = _lightSourceMesh;
    }

    public void SetLightParam(bool isFocused)
    {
        _focusSeq.Kill();
        _focusSeq = DOTween.Sequence();

        _focusSeq.Insert(0, DOVirtual.Float(m_light.innerSpotAngle,
            isFocused ? _focusedParams.ConeRanges.x : _defaultParams.ConeRanges.x, 0.5f,
            a =>
            {
                m_light.innerSpotAngle = a;
            }));
        
        _focusSeq.Insert(0, DOVirtual.Float(m_light.spotAngle,
            isFocused ? _focusedParams.ConeRanges.y : _defaultParams.ConeRanges.y, 0.5f,
            a =>
            {
                m_light.spotAngle = a;
            }));
        
        _focusSeq.Insert(0, DOVirtual.Float(m_light.range,
            isFocused ? _focusedParams.Range : _defaultParams.Range, 0.5f,
            a =>
            {
                m_light.range = a;
            }));
        
        _focusSeq.Insert(0, DOVirtual.Float(m_light.intensity,
            isFocused ? _focusedParams.Intensity : _defaultParams.Intensity, 0.5f,
            a =>
            {
                if (_hasMesh) _lightSourceMesh.UpdateLight();
                m_light.intensity = a;
            }));
    }

    private void SetRotation()
    {
        //add delayed ease to the light following the camera. 
        //no light must go towards camera direction but a bit earlier.
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
        
        _lightSourceMesh.gameObject.SetActive(m_isOn);
        m_light.intensity = m_isOn ? _defaultParams.Intensity : 0;
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
       m_light.intensity = _defaultParams.Intensity;
    }
}
