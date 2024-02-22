using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Game.World.Objects;
using UnityEngine;

namespace Game.Character
{
    public class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private float _interactibleDistance = 1;
        
        private PlayerController m_chrController;
        private Camera m_cam;
        private bool m_interacting;
        private IInteractable m_interactionTarget;
        private IInteractable m_focusTarget;
        private MouseInteractionStats m_startStats;
        private MouseInteractionStats m_endStats;

        [SerializeField] private MouseCamLook _mouseCamController;
        [SerializeField] private GameplayPanelUi _gameplayUI;
        [SerializeField] private bool _isDebugging;

        private void Start()
        {
            m_chrController = GetComponent<PlayerController>();
            m_cam = m_chrController.GameCam;
            m_focusTarget = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            Observe();
            CheckAndInteract();
            HandleUI();
         
            
            if (_isDebugging) OnDebugActive();
        }
        
        private void OnDebugActive()
        {
            Ray observerRay = m_cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var rayColor = Color.white;

            rayColor = m_focusTarget == null ? Color.white : Color.yellow;
            if (m_interacting) rayColor = Color.red;
            
            Debug.DrawRay(m_cam.transform.position, observerRay.direction * 100, rayColor, .15f);
        }

        private CrosshairStates m_cachedState = CrosshairStates.InActive;
        
        private void HandleUI()
        {
            var state = m_focusTarget == null ? CrosshairStates.InActive : CrosshairStates.Active;
            
            if (m_interacting) state = CrosshairStates.InUse;
            if (m_cachedState == state) return;
            
            m_cachedState = state;
            _gameplayUI.SetCrosshairState(state);
        }

        private void Observe()
        {
            Ray observerRay = m_cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit observerHit;
            
            
            if (Physics.Raycast(observerRay, out observerHit, _interactibleDistance))
            {
                if (m_focusTarget == null)
                {
                    m_focusTarget = observerHit.collider.GetComponent<IInteractable>();
                    
                    var obj = m_focusTarget?.GetInteractionGameObject();
                    
                    if (obj != null)
                        ScreenDubegger._objectInFocusDebug = "Focusing at " + obj.name;
                }
            }
            else
            {
                if (m_interacting) return;

                ScreenDubegger._objectInFocusDebug = "";
                m_focusTarget = null;
            }
        }

        private void CheckAndInteract()
        {
            if (m_cam != null && Input.GetMouseButtonDown(0))
            {
                Ray ray = m_cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _interactibleDistance))
                {
                    IInteractable interactableComponent = m_focusTarget;
                    m_interacting = interactableComponent != null;

                    if (m_interacting && m_interactionTarget == null)
                    {
                        _mouseCamController.SetFocusMode(true);
                        
                        m_interactionTarget = interactableComponent;
                        m_startStats = new MouseInteractionStats(Time.time, Input.mousePosition);
                        m_interactionTarget?.InteractStart(m_startStats);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && m_interactionTarget != null)
            {
                _mouseCamController.SetFocusMode(false);
                m_endStats = new MouseInteractionStats(Time.time, Input.mousePosition);
                m_interactionTarget.InteractEnd(m_endStats);
                m_interactionTarget = null;
                m_interacting = false;
            }
        }

        private void Callback()
        {
            throw new NotImplementedException();
        }
    }
}
