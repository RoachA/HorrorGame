using System;
using System.Collections;
using System.Collections.Generic;
using Game.World.Objects;
using UnityEngine;

namespace Game.Character
{
    public class InteractionController : MonoBehaviour
    {
        private CharacterController m_chrController;
        private Camera m_cam;

        private void Start()
        {
            m_chrController = GetComponent<CharacterController>();
            m_cam = m_chrController.GameCam;
        }

        private void Update()
        {
            if (m_cam != null && Input.GetMouseButtonDown(0))
            {
                Ray ray = m_cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log("Hit object: " + hit.collider.gameObject.name);
                    
                    if (hit.collider.GetComponent<IInteractable>() != null)
                    {
                        Debug.LogWarning(hit.collider.name + " is interactable!");
                    }
                }
            }
        }
    }
}
