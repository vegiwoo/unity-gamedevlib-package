using GameDevLib.Args;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Events;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    [RequireComponent(typeof(CharacterController), typeof(Animator), typeof(AudioIsPlaying))]
    public class HeroMovement : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links

        [Header("Input")] 
        [SerializeField] private InputManagerEvent inputManagerEvent;

        private Character _character;
        private Camera _mainCamera;
        private CharacterController _characterController;
        private Animator _animator;
        private AudioIsPlaying _audioIsPlaying;
        #endregion

        #region Constants and variables

        private float _currentSpeed;
        [CanBeNull] private InputManagerArgs _args;

        // player 
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private const float TerminalVelocity = 53.0f;
        
        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _mainCamera = Camera.main;
            _character = GetComponent<Character>();
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _audioIsPlaying = GetComponent<AudioIsPlaying>();
        }

        private void Start()
        {
            // reset our timeouts on start
            _jumpTimeoutDelta = _character.Stats.JumpTimeout;
            _fallTimeoutDelta = _character.Stats.FallTimeout;

            AssignAnimationIDs();
        }

        private void Update()
        {
            OnMovement();
        }

        private void OnEnable()
        {
            inputManagerEvent.Attach(this);
        }

        private void OnDisable()
        {
            inputManagerEvent.Detach(this);
        }

        #endregion

        #region Functionality

        private void OnMovement()
        {
            if (_args == null || _args.Moving == null) return;
            
            var stats = _character.Stats;
            
            // Walking or running character
            // set target speed based on move speed, sprint speed and if sprint is pressed
            // if there is no input, set the target speed to 0
            float targetSpeed = default;
            if (_args.Moving != Vector2.zero)
            {
                targetSpeed = _args.Running != null && _args.Running.Value ? stats.MoveSpeed * _character.Stats.AccelerationFactor : stats.MoveSpeed;
            }
            
            // a reference to the players current horizontal velocity
            var velocity = _characterController.velocity;
            var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
            
            var speedOffset = 0.1f;
            var inputMagnitude = 1f;
            
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * stats.SpeedChangeRate);
                
                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * stats.SpeedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            // normalise input direction
            var inputDirection = new Vector3(_args.Moving.Value.x, 0.0f, _args.Moving.Value.y).normalized;

            // if there is a move input rotate player when the player is moving
            if (_args.Moving.Value != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    stats.RotationSmoothTime);
                
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            
            // update moving
            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                      new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            
            // update animator
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
        
        //
        // private void OnMovement()
        // {
        //     if(_args == null) return;
        //     
        //     // Grounded player
        //     _groundedPlayer = _characterController.isGrounded;
        //     if (_groundedPlayer && _playerVelocity.y < 0)
        //     {
        //         _playerVelocity.y = 0f;
        //     }
        //
        //     // Moving player
        //     if (_args.Moving != null)
        //     {
        //         var move = new Vector3(_args.Moving.Value.x, 0, _args.Moving.Value.y);
        //         var camTransform = _mainCamera.transform;
        //         move = move.x * camTransform.right.normalized + move.z * camTransform.forward.normalized;
        //         move.y = 0f;
        //
        //         _currentSpeed = _character.Stats.MoveSpeed;
        //         if (_args.Running != null && _args.Running.Value)
        //         {
        //             _currentSpeed += _character.Stats.AccelerationFactor;
        //         }
        //         
        //         _characterController.Move(move * (_currentSpeed * Time.deltaTime));
        //     }
        //
        //     // Jumping player
        //     if (_args.Jumping is true && _groundedPlayer)
        //     {
        //         _playerVelocity.y += Mathf.Sqrt(_character.Stats.JumpHeight * -3.0f * Data.Gravity);
        //     }
        //     
        //     _playerVelocity.y += Data.Gravity * Time.deltaTime;
        //     _characterController.Move(_playerVelocity * Time.deltaTime);
        //     
        //     // Rotate towards camera direction 
        //     var rotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
        //     transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _character.Stats.BaseRotationSpeed * Time.deltaTime);
        // }
        // public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        // {
        //     _args = args;
        // }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

            #endregion
        }

        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            _args = args;
        }

        private void OnFootstep()
        {
            _audioIsPlaying.PlaySound(SoundType.RandomFromArray);
        }
    }
}