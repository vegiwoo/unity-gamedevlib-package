using System;
using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    public class CharacterShooting : MonoBehaviour, Interfaces.IObserver<InputManagerArgs>
    {
        #region Links
        [field: SerializeField, Tooltip("Layer taken into account when aiming")]
        private LayerMask LayerMask { get; set; }
        [field: SerializeField, Tooltip("Sight beam color (for Gismo)")]
        private Color AimColor { get; set; }
        
        [SerializeField] 
        private InputManagerEvent inputEvent;
        
        private Weapon.Weapon _currentWeapon;
        private Camera _mainCamera;
        #endregion
        
        #region 
        private Vector3 _aimPoint;
        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            _mainCamera = Camera.main;
            _currentWeapon = GetComponentInChildren<Weapon.Weapon>();
        }

        private void Start()
        {
            _aimPoint = Vector3.zero;
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
            OnTakeAim();
        }

        private void OnDrawGizmos()
        {
            if(_mainCamera == null) return;
             
            var point01 = _mainCamera.transform.position;
            var point02 = _aimPoint;
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(point01, 0.2f);
            Gizmos.DrawSphere(point02, 0.2f);
            Gizmos.DrawLine(point01, point02);
        }

        #endregion
        
        #region Functionality
        
        private void OnTakeAim()
        {
            var camTransform = _mainCamera.transform;

            var hit = AimingRaycast(camTransform.position, camTransform.forward);
            var point = hit.point;
            if(point == default) return;
            
            var targetDistance = Vector3.Distance( _currentWeapon.ShotPoint.position, point);
            if (targetDistance < _currentWeapon.stats.ShotRange)
            {
                _aimPoint = point;
            }
            else
            {
                _aimPoint = camTransform.position + camTransform.forward * _currentWeapon.stats.ShotRange;
            }
        }
        
        /// <summary>
        /// Performs a raycast of character's aiming.
        /// </summary>
        /// <param name="origin">Initial aiming point.</param>
        /// <param name="direction">Aiming direction</param>
        /// <param name="layerType">Type of interaction with passed LayerMask (interaction or ignorance LayerMask).</param>
        /// <param name="maxDistance">Maximum aiming distance.</param>
        /// <returns>Result of aiming.</returns>
        private RaycastHit AimingRaycast(Vector3 origin, Vector3 direction, RaycastLayerType? layerType = null,
            float maxDistance = Mathf.Infinity)
        {
            RaycastHit hit;
            
            if (layerType != null)
            {
                Physics.Raycast(
                    origin, 
                    direction, 
                    out hit, 
                    maxDistance, 
                    layerType == RaycastLayerType.Ignorance ? ~LayerMask : LayerMask
                );
            }
            else
            {
                Physics.Raycast(
                    origin,
                    direction,
                    out hit,
                    maxDistance);
            }

            return hit;
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if (args.Firing)
            {
                _currentWeapon.Fire(_aimPoint);
            }
        }
        #endregion
    }
}