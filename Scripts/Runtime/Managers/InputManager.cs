using System.Collections.Generic;
using GameDevLib.Args;
using GameDevLib.Events;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Managers
{
    public enum InputKey
    {
        Move, Run, Look, Jump, Aim, Light,Fire, Fling, Using
    }

    public class InputManager : MonoBehaviour
    {
        #region Links
        [SerializeField] private InputManagerEvent inputManagerEvent;
        private PlayerInput _playerInput;
        #endregion

        #region Constants and variables
        
        private readonly Dictionary<InputKey, InputAction> _actions = new ();
        
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

        #endregion
        
        #region MonoBehaviour methods
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _actions[InputKey.Move] = _playerInput.actions[InputKey.Move.ToString()];
            _actions[InputKey.Run] = _playerInput.actions[InputKey.Run.ToString()];
            _actions[InputKey.Look] = _playerInput.actions[InputKey.Look.ToString()];
            _actions[InputKey.Jump] = _playerInput.actions[InputKey.Jump.ToString()];
            _actions[InputKey.Aim] = _playerInput.actions[InputKey.Aim.ToString()];
            _actions[InputKey.Light] = _playerInput.actions[InputKey.Light.ToString()];
            _actions[InputKey.Fire] = _playerInput.actions[InputKey.Fire.ToString()];
            _actions[InputKey.Fling] = _playerInput.actions[InputKey.Fling.ToString()];
            _actions[InputKey.Using] = _playerInput.actions[InputKey.Using.ToString()];
        }
        
        private void Start()
        {
            _movingDestination = Vector2.zero;
            _isRunning = _isJumping = false;
            _isAiming = null;
        }

        private void OnEnable()
        {
            _actions[InputKey.Move].performed += Moving;
            _actions[InputKey.Move].canceled += Moving;
            
            _actions[InputKey.Run].performed += Running;
            _actions[InputKey.Run].canceled += Running;

            _actions[InputKey.Jump].performed += Jumping;
            _actions[InputKey.Jump].canceled += Jumping;
            
            _actions[InputKey.Aim].performed += Aiming;
            _actions[InputKey.Aim].canceled += Aiming;
            
            _actions[InputKey.Light].performed += Lighting;
            _actions[InputKey.Light].canceled += Lighting;
        }
        
        private void OnDisable()
        {
            _actions[InputKey.Move].performed -= Moving;
            _actions[InputKey.Move].canceled -= Moving;
            
            _actions[InputKey.Run].performed -= Running;
            _actions[InputKey.Run].canceled -= Running;

            _actions[InputKey.Jump].performed -= Jumping;
            _actions[InputKey.Jump].canceled -= Jumping;
            
            _actions[InputKey.Aim].performed -= Aiming;
            _actions[InputKey.Aim].canceled -= Aiming;
            
            _actions[InputKey.Light].performed -= Lighting;
            _actions[InputKey.Light].canceled -= Lighting;
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
            
            _isJumping = _actions[InputKey.Jump].triggered;
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
            _isLighting = context.performed;
            Notify();
        }

        private void Notify()
        {
            var args = new InputManagerArgs( _movingDestination, _isRunning, _isJumping, _isAiming, _isLighting);
            inputManagerEvent.Notify(args);
        }
        #endregion
    }
}