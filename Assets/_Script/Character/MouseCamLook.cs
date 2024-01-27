using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    public GameObject character;
    private Vector2 m_mouseLook;
    private Vector2 m_smoothV;
    private bool _camLookActive;

    // Use this for initialization
    private void Start() 
    {
        character = this.transform.parent.gameObject;
    }
	
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _camLookActive = !_camLookActive;

        Cursor.lockState = _camLookActive ? CursorLockMode.Locked : CursorLockMode.None;
        
        if (_camLookActive == false) return;
        
        Cursor.lockState = CursorLockMode.Locked;
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        m_smoothV.x = Mathf.Lerp(m_smoothV.x, md.x, 1f / smoothing);
        m_smoothV.y = Mathf.Lerp(m_smoothV.y, md.y, 1f / smoothing);
        m_mouseLook += m_smoothV;
        
        transform.localRotation = Quaternion.AngleAxis(-m_mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(m_mouseLook.x, character.transform.up);
    }
}
