using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
                if (instance == null)
                {
                    Debug.LogError("Player instance not found in the scene.");
                }
            }

            return instance;
        }
    }
    
    public float Speed = 10.0f;
    private float m_translation;
    private float m_strafe;
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private Camera _gameCam;
    [SerializeField] private bool _camShakeActive = false;
    [SerializeField] private float _camShakeMultiplier = 1f;

    private CinemachineBasicMultiChannelPerlin _noiseShakeComponent;
    
    public CinemachineVirtualCamera VCam
    {
        get { return _vCam; }
    }

    public Camera GameCam
    {
        get { return _gameCam;  }
    }
    
    // Use this for initialization
    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        _noiseShakeComponent = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
	
    // Update is called once per frame
    private void Update()
    {
        m_translation = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        m_strafe = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        transform.Translate(m_strafe, 0, m_translation);

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (_camShakeActive)
            _noiseShakeComponent.m_AmplitudeGain = m_translation * _camShakeMultiplier + 1;
        // _noiseShakeComponent.m_FrequencyGain = m_translation * _camShakeMultiplier * 0.25f + 0.25f;
    }
}
