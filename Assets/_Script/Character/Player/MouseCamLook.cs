using System;
using Cinemachine;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    [BoxGroup("References")]
    public CharacterController Character;
    [BoxGroup("References")]
    public CinemachineVirtualCamera VCam;
    [Header("Character Look Settings")]
    [SerializeField] private Vector2 sensitivityRange;
    private float m_sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;

    [Header("Bobbing Settings")]
    [SerializeField] private BobbingSetting[] _bobbingProfiles;
    private BobbingSetting m_activeBobbingSetting;
    
    private Vector2 m_mouseLook;
    private Vector2 m_smoothV;
    private bool _camLookActive = true;
    private bool m_walkBobbing = false;

    private Vector3 m_camStartPos;
    private CinemachineBasicMultiChannelPerlin m_noiseShakeComponent;
    private Vector2 m_noiseShakeBaseVal = new Vector2();
    

    // Use this for initialization
    private void Start() 
    {
        if (Character == null)
            Character = transform.GetComponentInParent<CharacterController>();
        
        m_noiseShakeComponent = VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_noiseShakeBaseVal.x = m_noiseShakeComponent.m_AmplitudeGain;
        m_noiseShakeBaseVal.y = m_noiseShakeComponent.m_FrequencyGain;
        
        m_camStartPos = transform.localPosition;
        transform.rotation = quaternion.Euler(Vector3.zero);
    }

    public void SetFocusMode(bool isFocusMode)
    {
        m_sensitivity = isFocusMode ? sensitivityRange.x : sensitivityRange.y;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _camLookActive = !_camLookActive;
        
        if (_camLookActive == false) return;
        
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(m_sensitivity * smoothing, m_sensitivity * smoothing));
        m_smoothV.x = Mathf.Lerp(m_smoothV.x, md.x, 1f / smoothing);
        m_smoothV.y = Mathf.Lerp(m_smoothV.y, md.y, 1f / smoothing);
        m_mouseLook += m_smoothV;
        m_mouseLook.y = Mathf.Clamp(m_mouseLook.y, -60, 75);
        
       
        transform.localRotation = Quaternion.AngleAxis(-m_mouseLook.y, Vector3.right);
        Character.transform.localRotation = Quaternion.AngleAxis(m_mouseLook.x, Character.transform.up);

        CheckMotion();
    }
    
    private bool m_isMoving = false;
    private BobbingSetting m_previousSetting;
    private Vector2 m_previousFrameNoiseProfile;
    
    private void CheckMotion()
    {
        var speed = new Vector3(Character.velocity.x, 0, Character.velocity.z).magnitude;
        m_isMoving = speed >= 0.1f;
        
        if (m_isMoving == false)
        {
            ResetBobbingPosition();
            ResetToOriginalShakeProfile();
            return;
        }
         //   if (Character.isGrounded == false) return;

        foreach (var profile in _bobbingProfiles)
        {
            if (speed > profile.ToggleSpeed)
            {
                m_activeBobbingSetting = profile;
            }
        }
        
        //this ensures if the bobbing type is new, it starts a new lerp with CameraShakeMotion(); //todo a bit ugly no?
        if (Math.Abs(m_previousSetting.ToggleSpeed - m_activeBobbingSetting.ToggleSpeed) > 0.1f) m_camShakeCurrentTime = 0;
            
        transform.localPosition += FootStepMotion();
        CameraShakeMotion();
        m_previousFrameNoiseProfile = new Vector2(m_noiseShakeComponent.m_AmplitudeGain, m_noiseShakeComponent.m_FrequencyGain);
        m_camShakeResetCurrentTime = 0;
    }

    private void ResetBobbingPosition()
    {
        if (transform.localPosition == m_camStartPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_camStartPos, 2 * Time.deltaTime);
    }

    private float m_camShakeCurrentTime = 0;
    private float m_camShakeLerpDuration = 1f;
    
    private void CameraShakeMotion()
    {
        if (m_camShakeCurrentTime >= m_camShakeLerpDuration || m_isMoving == false)
        {
           return;
        }
        
        float amplitudeDelta = 0;
        float freqDelta = 0;
        
        m_camShakeCurrentTime += Time.deltaTime;
        m_camShakeCurrentTime = Mathf.Clamp(m_camShakeCurrentTime, 0f, m_camShakeLerpDuration);
        float t = m_camShakeCurrentTime / m_camShakeLerpDuration;
        
        amplitudeDelta = Mathf.Lerp(0, m_activeBobbingSetting.CameraShakeAmplitude, t);
        freqDelta = Mathf.Lerp(0, m_activeBobbingSetting.CameraShakeFrequency, t);
        m_noiseShakeComponent.m_AmplitudeGain = m_noiseShakeBaseVal.x + amplitudeDelta;
        m_noiseShakeComponent.m_FrequencyGain = m_noiseShakeBaseVal.y + freqDelta;
    }

    private float m_camShakeResetCurrentTime;
    
    private void ResetToOriginalShakeProfile()
    {
        float amplitudeDelta = 0;
        float freqDelta = 0;
        
        m_camShakeResetCurrentTime += Time.deltaTime;
        m_camShakeResetCurrentTime = Mathf.Clamp(m_camShakeCurrentTime, 0f, 1);
        float t = m_camShakeResetCurrentTime / 1;
        
        amplitudeDelta = Mathf.Lerp(m_previousFrameNoiseProfile.x, m_noiseShakeBaseVal.x, t);
        freqDelta = Mathf.Lerp(m_previousFrameNoiseProfile.y, m_noiseShakeBaseVal.y, t);
        
        m_noiseShakeComponent.m_AmplitudeGain = amplitudeDelta;
        m_noiseShakeComponent.m_FrequencyGain = freqDelta;
        
        m_camShakeCurrentTime = 0;
    }
    
    private const float PerlinSampleFrequency = 5f;
    private readonly Vector2 m_offset = Vector2.zero;

    private Vector3 FootStepMotion()
    {
        var animatedPerlinValue = 0.01f;
        
        if (m_activeBobbingSetting.UsePerlinNoise)
        {
            float time = Time.deltaTime * PerlinSampleFrequency; 
            animatedPerlinValue = Mathf.PerlinNoise(m_offset.x + time, m_offset.y + time);
        }
        
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * m_activeBobbingSetting.Frequency) * m_activeBobbingSetting.Amplitude;
        pos.x += Mathf.Cos(Time.time * m_activeBobbingSetting.Frequency / animatedPerlinValue) * m_activeBobbingSetting.Amplitude * animatedPerlinValue;
        return pos;
    }
    
    [Serializable]
    public struct BobbingSetting
    {
        [Header("Position Bobbing")]
        [Range(0, 10)] public float ToggleSpeed;
        [Range(0, 0.2f)] public float Amplitude;
        [Range(0, 30)] public float Frequency;
        public bool UsePerlinNoise;
        [Header("Camera Position Noise")]
        [Range(0, 10)] public float CameraShakeAmplitude;
        [Range(0, 10)] public float CameraShakeFrequency;
    }
}
