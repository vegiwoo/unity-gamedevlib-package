using System;
using UnityEngine;
using Cinemachine;
using GameDevLib.Args;
using GameDevLib.Events;
using UnityEngine.InputSystem;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Cameras {
    public class ThirdPersonCameraGroup : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links 
        [Header("Cameras")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineVirtualCamera vCamNormal;
        [SerializeField] private CinemachineVirtualCamera vCamAim;
        [Header("Following")]
        [SerializeField] private Transform cameraFollowing;
        [Header("Events")]
        [SerializeField] private InputManagerEvent inputManagerEvent;
        [Header("Others")]
        [SerializeField] private int priorityBoostAmount = 10;
        [SerializeField] private Canvas vCamNormalCanvas;
        [SerializeField] private Canvas vCamAimCanvas;
        #endregion
        
        private const int VCamNormalPriority = 10;
        private const int VCamFOV = 40;
        
        private const int VCamAimPriority = 9;
        
        private void Start()
        {
            const string helpString = 
                @"1. Add all cameras & camera following target to component;
                2. Add 'Environment' layer & add to 'vCam -> Cinemachine collider -> Collide Against';
                3. Add PlayerInput to vCamNormal -> Cinemachine Input Provider -> XY Axis;";
            Debug.LogWarning(helpString);

            // vCamNormal
            vCamNormal.m_Follow = cameraFollowing;
            vCamNormal.m_LookAt = cameraFollowing;
            vCamNormal.Priority = VCamNormalPriority;
            vCamNormal.m_Lens.FieldOfView = VCamFOV;

            // vCamAim
            vCamAim.m_Follow = cameraFollowing;
            vCamAim.m_LookAt = cameraFollowing;
            vCamAim.Priority = VCamAimPriority;
            vCamAim.m_Lens.FieldOfView = VCamFOV;
            
            vCamNormalCanvas.enabled = true;
            vCamAimCanvas.enabled = false;
        }

        private void OnEnable()
        {
            inputManagerEvent.Attach(this);
        }

        private void OnDisable()
        {
            inputManagerEvent.Detach(this);
        }

        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if(args.Aiming == null) return;

            switch (args.Aiming)
            {
                case true:
                    vCamAim.Priority += priorityBoostAmount;
                    vCamNormalCanvas.enabled = false;
                    vCamAimCanvas.enabled = true;
                    Debug.Log("Aiming!");
                    break;
                case false:
                    vCamAim.Priority -= priorityBoostAmount;
                    vCamAimCanvas.enabled = false;
                    vCamNormalCanvas.enabled = true;
                    Debug.Log("No aiming!");
                    break;
            }
        }
    }
}



