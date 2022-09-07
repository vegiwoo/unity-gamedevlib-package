using GameDevLib.Args;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    [RequireComponent(typeof(CharacterController), typeof(Animator), typeof(AudioIsPlaying))]
    public class HeroMovement : MonoBehaviour, Interfaces.IObserver<InputManagerArgs>, IAnimatorParametersWorkable
    {
        #region Links
        [Header("Input")] 
        [SerializeField] private InputManagerEvent inputManagerEvent;

        private Character _character;
        private Camera _mainCamera;
        private CharacterController _characterController;
        private AudioIsPlaying _audioIsPlaying;
        #endregion

        #region Constants and variables

        private float _currentSpeed;
        [CanBeNull] private InputManagerArgs _args;

        // player 
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private const float TerminalVelocity = 53.0f;
        
        [field: SerializeField, ReadonlyField]
        private bool isGrounded = true;

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
            GroundedCheck();
            OnMovement();
            JumpAndGravity();
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

        private void GroundedCheck()
        {
            var stats = _character.Stats;
            var position = transform.position;
            // set sphere position, with offset
            var spherePosition = new Vector3(position.x, position.y - stats.GroundedOffset, position.z);
            isGrounded = Physics.CheckSphere(spherePosition, stats.GroundedRadius, stats.GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator 
            _character.Animator.SetBool(_animIDGrounded, isGrounded);
        }
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
            
            const float speedOffset = 0.1f;
            const float inputMagnitude = 1f;
            
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                _character.CurrentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * stats.SpeedChangeRate);
                
                // round speed to 3 decimal places
                _character.CurrentSpeed = Mathf.Round(_character.CurrentSpeed * 1000f) / 1000f;
            }
            else
            {
                _character.CurrentSpeed = targetSpeed;
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
            _characterController.Move(targetDirection.normalized * (_character.CurrentSpeed * Time.deltaTime) +
                                      new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            
            // update animator
            _character.Animator.SetFloat(_animIDSpeed, _animationBlend);
            _character.Animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        private void JumpAndGravity()
        {
            if(_args == null || _args.Jumping == null) return;
            
            var stats = _character.Stats;

            if (isGrounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = stats.FallTimeout;

                // update animator
                _character.Animator.SetBool(_animIDJump, false);
                _character.Animator.SetBool(_animIDFreeFall, false);
                
                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2.0f;
                }

                // Jump
                if (_args.Jumping.Value && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt( stats.JumpHeight * -2f * stats.Gravity);

                    // update animator
                    _character.Animator.SetBool(_animIDJump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = stats.JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    _character.Animator.SetBool(_animIDFreeFall, true);
                    
                }

                // if we are not grounded, do not jump
                _args.Jumping = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < TerminalVelocity)
            {
                _verticalVelocity += stats.Gravity * Time.deltaTime;
            }
        }
        
        public void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            _args = args;
        }

        /// <summary>
        /// Triggered when a character steps on ground, triggered via animation clip events.
        /// </summary>
        private void OnFootstep()
        {
            _audioIsPlaying.PlaySound(SoundType.RandomFromArray);
        }
        
        /// <summary>
        /// Triggered when the character hits the ground, triggered by animation clip events.
        /// </summary>
        private void OnLand()
        {
            _audioIsPlaying.PlaySound(SoundType.Positive);
        }
        #endregion
    }
}