using System;
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

    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private Camera _gameCam;
    [Header("Movement")]
    public float WalkingSpeed = 3f;
    public float RunningSpeed = 6f;
    private float m_translation;
    private float m_strafe;

    [Header("Camera")]
    [SerializeField] private bool _camShakeActive = false;
    [SerializeField] private float _camShakeMultiplier = 1f;

    [Header("Flash Light")]
    [SerializeField] private FlashLightHandler _flashLight;

    private CinemachineBasicMultiChannelPerlin _noiseShakeComponent;
    private CharacterController m_characterController;
    private bool _isSprinting;

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

        // Calculate movement inputs
        float translation = Input.GetAxis("Vertical") * (_isSprinting ? RunningSpeed : WalkingSpeed);
        float strafe = Input.GetAxis("Horizontal") * WalkingSpeed;

        // Apply movement using Rigidbody physics
        Vector3 movement = new Vector3(strafe * Time.fixedDeltaTime, 0, translation * Time.fixedDeltaTime);
        m_characterController.Move(transform.TransformDirection(movement));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (_camShakeActive)
        {
            // Adjust camera shake based on movement
            _noiseShakeComponent.m_AmplitudeGain = translation * _camShakeMultiplier + 1;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _flashLight.SwitchFlashlight();
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isSprinting = false;
        }
    }
}