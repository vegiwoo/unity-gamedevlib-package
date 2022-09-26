using UnityEngine;
using Cinemachine;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Interfaces;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Cameras {
    public class ThirdPersonCameraGroup : MonoBehaviour, Interfaces.IObserver<InputManagerArgs>
    {
        #region Links 
        [Header("Cameras")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CinemachineVirtualCamera vCamNormal;
        [SerializeField] private CinemachineVirtualCamera vCamAim;
        [Header("Following")]
        [SerializeField] private Transform cameraFollowing;
        [Header("Events")]
        [SerializeField] private InputEvent inputEvent;
        [Header("Others")]
        [SerializeField] private int priorityBoostAmount = 10;
        [SerializeField] private bool isShowAims;
        [SerializeField] private Canvas vCamNormalCanvas;
        [SerializeField] private Canvas vCamAimCanvas;

        private InputManagerArgs _args;
        #endregion
        
        #region Constants and variables
        private const int VCamNormalPriority = 10;
        private const int VCamAimPriority = 9;
        private const int VCamFOV = 40;
        #endregion
        
        #region MonoBehaviour methods
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

            // Hiding aims
            SettingDisplayCrosshairs();
        }

        private void OnEnable()
        {
            inputEvent.Attach(this);
        }

        private void OnDisable()
        {
            inputEvent.Detach(this);
        }
        
        private void Update()
        {
            SettingDisplayCrosshairs();
        }

        #endregion
        
        #region Functionality

        private void SettingDisplayCrosshairs()
        {
            if (!isShowAims)
            {
                vCamNormalCanvas.enabled = vCamAimCanvas.enabled = false;
                vCamNormal.Priority = VCamNormalPriority;
                vCamAim.Priority = VCamAimPriority;
            }
            else
            {
                if(_args == null) return;
                
                switch (_args.Aiming)
                {
                    case true:
                        vCamAim.Priority = VCamAimPriority + priorityBoostAmount;
                        vCamNormalCanvas.enabled = false;
                        vCamAimCanvas.enabled = true;
                        break;
                    case false:
                        vCamAim.Priority = VCamAimPriority;
                        vCamAimCanvas.enabled = false;
                        vCamNormalCanvas.enabled = true;
                        break;
                }
            }
        }
        
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            _args = args;
        }
        #endregion
    }
}