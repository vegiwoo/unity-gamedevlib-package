using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links
        [Header("Stats")] 
        [SerializeField] private CharacterStats stats;
        [Header("Events")]
        [SerializeField] private InputManagerEvent inputManagerEvent;
        
        private Camera _mainCamera;
        private CharacterController _characterController;
        #endregion
        
        #region Constants and variables 
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;

        private float _currentHp;
        private float _currentSpeed;
        
        private Vector2 _moving = Vector2.zero;
        private bool _isRunning = false;
        private bool _isJumping = false;
        #endregion

        #region MonoBehaviour methods
        private void Awake()
        {
            _mainCamera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _currentHp = stats.MaxHp;
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
            // Grounded player
            _groundedPlayer = _characterController.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }
            
            // Moving player
            if (_moving != Vector2.zero)
            {
                var move = new Vector3(_moving.x, 0, _moving.y);
                
                var camTransform = _mainCamera.transform;
                move = move.x * camTransform.right.normalized + move.z * camTransform.forward.normalized;
                move.y = 0f;
                
                _currentSpeed = stats.BaseMovingSpeed;
                
                if (_isRunning)
                {
                    _currentSpeed += stats.AccelerationFactor;
                }
                
                _characterController.Move(move * (_currentSpeed * Time.deltaTime));
            }
            
            // Jumping player
            if (_isJumping && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(stats.JumpHeight * -3.0f * Data.Gravity);
                _playerVelocity.y += Data.Gravity * Time.deltaTime;
                _characterController.Move(_playerVelocity * Time.deltaTime);
                _isJumping = false;
            }
            
            // Rotate towards camera direction 
            var r = transform.rotation;
            var rotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(r, rotation, stats.BaseRotationSpeed * Time.deltaTime);
          
            
        }
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if (args.Moving != null)
            {
                _moving = (Vector2)args.Moving;
            }

            if (args.Running != null)
            {
                _isRunning = (bool)args.Running;
            }

            if (args.Jumping != null)
            {
                _isJumping = (bool)args.Jumping;
            }
            
        }
        #endregion

    }
}