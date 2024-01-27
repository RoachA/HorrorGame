using Cinemachine;
using Game.UI;
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
    
    [Header("Movement")]
    public float WalkingSpeed = 10.0f;
    public float RunningSpeed = 17f;
    private float m_translation;
    private float m_strafe;
    
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private Camera _gameCam;
    [SerializeField] private bool _camShakeActive = false;
    [SerializeField] private float _camShakeMultiplier = 2f;

    private CinemachineBasicMultiChannelPerlin _noiseShakeComponent; 
    private bool m_isSprinting;
    private CharacterController m_characterController;
    
    public CinemachineVirtualCamera VCam
    {
        get { return _vCam; }
    }

    public Camera GameCam
    {
        get { return _gameCam; }
    }
    
    // Use this for initialization
    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        _noiseShakeComponent = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_characterController = GetComponent<CharacterController>();
    }
	
    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleSprint();
        HandleCamShake();

        
        float translation = Input.GetAxis("Vertical") * (m_isSprinting ? RunningSpeed : WalkingSpeed);
        float strafe = Input.GetAxis("Horizontal") * WalkingSpeed;

        // Apply movement using Rigidbody physics
        Vector3 movement = new Vector3(strafe * Time.deltaTime, 0, translation * Time.fixedDeltaTime);
        m_characterController.Move(transform.TransformDirection(movement));

        ScreenDubegger._velocity = m_characterController.velocity.z.ToString("f4");
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HandleCamShake()
    {
        if (_camShakeActive == false) return;
        
        if (_camShakeActive)
        {
            _noiseShakeComponent.m_AmplitudeGain = 1 + m_characterController.velocity.magnitude;
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_isSprinting = false;
        }
    }
}
