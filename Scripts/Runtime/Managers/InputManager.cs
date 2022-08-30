using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        #region Links
        [SerializeField] private InputManagerEvent inputManagerEvent;
        private PlayerInput _playerInput;
        #endregion

        #region Constants and variables
        
        private readonly Dictionary<InputManagerKey, InputAction> _actions = new ();
        
        private InputAction _moveAction;
        private InputAction _runAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _aimAction;
        private InputAction _lightAction;
        private InputAction _fireAction;
        private InputAction _flingAction;
        private InputAction _usingAction;
        
        private Vector2 _movingDestination;
        private bool _isRunning;
        private bool _isJumping;
        private bool? _isAiming;
        private bool? _isLighting;
        private bool? _isFiring;

        #endregion
        
        #region MonoBehaviour methods
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _actions[InputManagerKey.Move] = _playerInput.actions[InputManagerKey.Move.ToString()];
            _actions[InputManagerKey.Run] = _playerInput.actions[InputManagerKey.Run.ToString()];
            _actions[InputManagerKey.Look] = _playerInput.actions[InputManagerKey.Look.ToString()];
            _actions[InputManagerKey.Jump] = _playerInput.actions[InputManagerKey.Jump.ToString()];
            _actions[InputManagerKey.Aim] = _playerInput.actions[InputManagerKey.Aim.ToString()];
            _actions[InputManagerKey.Light] = _playerInput.actions[InputManagerKey.Light.ToString()];
            _actions[InputManagerKey.Fire] = _playerInput.actions[InputManagerKey.Fire.ToString()];
            _actions[InputManagerKey.Fling] = _playerInput.actions[InputManagerKey.Fling.ToString()];
            _actions[InputManagerKey.Using] = _playerInput.actions[InputManagerKey.Using.ToString()];
        }
        
        private void Start()
        {
            _movingDestination = Vector2.zero;
            _isRunning = _isJumping = false;
            _isAiming = _isLighting = _isFiring = null;
        }

        private void OnEnable()
        {
            _actions[InputManagerKey.Move].performed += Moving;
            _actions[InputManagerKey.Move].canceled += Moving;
            
            _actions[InputManagerKey.Run].performed += Running;
            _actions[InputManagerKey.Run].canceled += Running;

            _actions[InputManagerKey.Jump].performed += Jumping;
            _actions[InputManagerKey.Jump].canceled += Jumping;
            
            _actions[InputManagerKey.Aim].performed += Aiming;
            _actions[InputManagerKey.Aim].canceled += Aiming;
            
            _actions[InputManagerKey.Light].performed += Lighting;

            _actions[InputManagerKey.Fire].performed += Firing;
        }
        
        private void OnDisable()
        {
            if(_actions == null || _actions.Count == 0) return;
            
            _actions[InputManagerKey.Move].performed -= Moving;
            _actions[InputManagerKey.Move].canceled -= Moving;
            
            _actions[InputManagerKey.Run].performed -= Running;
            _actions[InputManagerKey.Run].canceled -= Running;

            _actions[InputManagerKey.Jump].performed -= Jumping;
            _actions[InputManagerKey.Jump].canceled -= Jumping;
            
            _actions[InputManagerKey.Aim].performed -= Aiming;
            _actions[InputManagerKey.Aim].canceled -= Aiming;
            
            _actions[InputManagerKey.Light].performed -= Lighting;
            
            _actions[InputManagerKey.Fire].performed -= Firing;
        }
        #endregion
        
        #region Functionality
        private void Moving(InputAction.CallbackContext context)
        {
            if(context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled)) return;
            
            _movingDestination = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
            Notify();
        }

        private void Running(InputAction.CallbackContext context)
        {
            if(context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled)) return;
            
            _isRunning = context.performed;
            Notify();
        }

        private void Jumping(InputAction.CallbackContext context)
        {
            if(context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled)) return;
            
            _isJumping = _actions[InputManagerKey.Jump].triggered;
            Notify();
        }
        
        private void Aiming(InputAction.CallbackContext context)
        {
            if (context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled))
            {
                _isAiming = null;  
                return;
            }
            _isAiming = context.performed; 
            Notify();
        }

        private void Lighting(InputAction.CallbackContext context)
        {
            if (context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled))
            {
                _isLighting = null;
                return;
            }
            _isLighting = context.action.triggered;
            Notify();
        }

        private void Firing(InputAction.CallbackContext context)
        {
            if (context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled))
            {
                _isFiring = null;
                return;
            }
            
            _isFiring = _actions[InputManagerKey.Fire].triggered;
            Notify();
        }

        private void Notify()
        {
            var args = new InputManagerArgs( _movingDestination, _isRunning, _isJumping, _isAiming, _isLighting, _isFiring);
            inputManagerEvent.Notify(args);
        }
        #endregion
    }
}