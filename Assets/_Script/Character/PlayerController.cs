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
    private CharacterController m_characterController;
    
    [Header("Flash Light")]
    [SerializeField] private FlashLightHandler _flashLight;
    
    private bool m_isSprinting;
    private Vector3 m_moveVector;

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
        m_characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleSprint();

        // Calculate movement inputs
        float translation = Input.GetAxis("Vertical") * (m_isSprinting ? RunningSpeed : WalkingSpeed);
        float strafe = Input.GetAxis("Horizontal") * WalkingSpeed;
        
        Vector3 movement = new Vector3(strafe * Time.fixedDeltaTime, 0, translation * Time.fixedDeltaTime);
        m_characterController.Move(transform.TransformDirection(movement));
        
        m_moveVector = Vector3.zero;
        
        if (m_characterController.isGrounded == false)
        {
            m_moveVector += Physics.gravity;
        }
        
        m_characterController.Move(m_moveVector * Time.deltaTime);
    }

    private void Update()
    {
        HandleFlashLight();
    }

    private void HandleFlashLight()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _flashLight.SwitchFlashlight();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _flashLight.SetLightParam(true);
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            _flashLight.SetLightParam(false);
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