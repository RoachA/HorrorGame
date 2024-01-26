using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Game.World.Objects;
using UnityEngine;

namespace Game.Character
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private float _interactibleDistance = 1;
        
        private CharacterController m_chrController;
        private Camera m_cam;
        private bool m_interacting;
        private IInteractable m_interactionTarget;
        private InteractionStat m_startStat;
        private InteractionStat m_endStat;

        [SerializeField] private GameplayPanelUi _gameplayUI;

        private void Start()
        {
            m_chrController = GetComponent<CharacterController>();
            m_cam = m_chrController.GameCam;
        }

        private void Update()
        {
            //todo can show some crosshair too. Laters.
            if (m_cam != null && Input.GetMouseButtonDown(0))
            {
                Ray ray = m_cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _interactibleDistance))
                {
                    IInteractable interactableComponent = hit.collider.GetComponent<IInteractable>();
                    m_interacting = interactableComponent != null;
                    _gameplayUI.SetCrosshairState(CrosshairStates.Active);

                    if (m_interacting && m_interactionTarget == null)
                    {
                        m_interactionTarget = interactableComponent;
                        m_startStat = new InteractionStat(Time.time, Input.mousePosition);
                        m_interactionTarget?.InteractStart(m_startStat);
                        _gameplayUI.SetCrosshairState(CrosshairStates.InUse);
                        Debug.Log("Started interacting with " + hit.collider.gameObject.name + " : : : " + m_startStat.Time);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && m_interactionTarget != null)
            {
                m_endStat = new InteractionStat(Time.time, Input.mousePosition);
                m_interactionTarget.InteractEnd(m_endStat);
                Debug.Log("Finished interacting with " + m_interactionTarget.GetInteractionGameObject().name + " : : : " + m_endStat.Time);
                m_interactionTarget = null;
                m_interacting = false;
                _gameplayUI.SetCrosshairState(CrosshairStates.InActive);
            }
        }
    }
}
