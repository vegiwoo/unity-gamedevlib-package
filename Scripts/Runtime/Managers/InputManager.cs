using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Args;
using GameDevLib.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using GameDevLib.Helpers;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Managers
{
    public enum InputKey
    {
        None, Move, Run, Look, Jump, Aim, Fire, Fling, Using
    }

    public class InputManager : MonoBehaviour
    {
        #region Links
        [SerializeField] private InputManagerEvent inputManagerEvent;
        #endregion

        private PlayerInput _playerInput;
        
        private readonly Dictionary<InputKey, InputAction> _actions = new ();
        
        private InputAction _moveAction;
        private InputAction _runAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _aimAction;
        private InputAction _fireAction;
        private InputAction _flingAction;
        private InputAction _usingAction;
        
        private Vector2 _moving;
        private bool _isRunning;
        private bool _isJumping;
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _actions[InputKey.Move] = _playerInput.actions[InputKey.Move.ToString()];
            _actions[InputKey.Run] = _playerInput.actions[InputKey.Run.ToString()];
            _actions[InputKey.Look] = _playerInput.actions[InputKey.Look.ToString()];
            _actions[InputKey.Jump] = _playerInput.actions[InputKey.Jump.ToString()];
            _actions[InputKey.Aim] = _playerInput.actions[InputKey.Aim.ToString()];
            _actions[InputKey.Fire] = _playerInput.actions[InputKey.Fire.ToString()];
            _actions[InputKey.Fling] = _playerInput.actions[InputKey.Fling.ToString()];
            _actions[InputKey.Using] = _playerInput.actions[InputKey.Using.ToString()];
        }
        
        private void Start()
        {
            _moving = Vector2.zero;
            _isRunning = _isJumping = false;
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
        }

        private void Moving(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    _moving = context.ReadValue<Vector2>();
                    break;
                case InputActionPhase.Canceled:
                    _moving = Vector2.zero;
                    break;
            }
            var args = new InputManagerArgs(null, _moving, _isRunning, _isJumping);
            inputManagerEvent.Notify(args);
        }

        private void Running(InputAction.CallbackContext context)
        {
            if(context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled)) return;
            
            _isRunning = context.performed;
            var args = new InputManagerArgs(null, _moving, _isRunning, _isJumping);
            inputManagerEvent.Notify(args);
        }

        private void Jumping(InputAction.CallbackContext context)
        {
            if(context.phase is not (InputActionPhase.Performed or InputActionPhase.Canceled)) return;

            _isJumping = _actions[InputKey.Jump].triggered;
            var args = new InputManagerArgs(null, _moving, _isRunning, _isJumping);
            inputManagerEvent.Notify(args);
        }
        
        private void Aiming(InputAction.CallbackContext context)
        {
            InputManagerArgs args = default;
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    args = new InputManagerArgs(true);
                    break;
                case InputActionPhase.Canceled:
                    args = new InputManagerArgs(false);
                    break;
            }
            inputManagerEvent.Notify(args);
        }
    }
}