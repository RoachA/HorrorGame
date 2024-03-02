using Cinemachine;
using Game.UI;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance; //todo only for debug
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

    [Inject] private readonly AudioManager _audioManager;
    [Inject] private readonly UIManager m_gameplayUIManager;
    
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
    
    [Header("Sfx")]
    [SerializeField] private float _footstepRate = 1.5f;
    
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
        m_floorMask = LayerMask.GetMask($"Floor");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleGravity();
        HandleWalk();
        HandleSprint();

        HandleFootstepSfx();
    }
    
    private void Update()
    {
        HandleFlashLight();
        HandleUIInteraction();
    }
    
    private float time = 0;
    private LayerMask m_floorMask;

    /// <summary>
    /// global UI entry is here for now!
    /// </summary>
    private void HandleUIInteraction()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (m_gameplayUIManager.TryClosePanel<InventoryPanel>()) return;
            m_gameplayUIManager.OpenPanel<InventoryPanel>(new UIPanelParams());
        }
    }
    
    private void HandleFootstepSfx()
    {
        time += Time.deltaTime;
       if (m_characterController.velocity.magnitude == 0) return;

       if (time > _footstepRate / m_characterController.velocity.magnitude)
       {
           RaycastHit hit;
           FloorSurfaceType floorType = FloorSurfaceType.Concrete;
           Debug.DrawRay(transform.position, Vector3.down * 5, Color.red, 1);
           if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, m_floorMask))
           {
               if (hit.collider.CompareTag("Floor"))
               {
                   floorType = hit.collider.gameObject.GetComponent<FloorHandler>().GetSurfaceType();
               }
           }
           
           time = 0;
           _audioManager.PlayFootstep(floorType, gameObject);
       }
    }

    private void HandleGravity()
    {
        if (m_characterController.isGrounded == false)
        {
            m_moveVector = Vector3.zero;
            m_moveVector += Physics.gravity;
            m_characterController.Move(m_moveVector * Time.deltaTime);
        }
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

    private void HandleWalk()
    {
        float translation = Input.GetAxis("Vertical") * (m_isSprinting ? RunningSpeed : WalkingSpeed);
        float strafe = Input.GetAxis("Horizontal") * WalkingSpeed;
        
        Vector3 movement = new Vector3(strafe * Time.fixedDeltaTime, 0, translation * Time.fixedDeltaTime);
        m_characterController.Move(transform.TransformDirection(movement));
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