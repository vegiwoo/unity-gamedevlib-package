using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using GameDevLib.Stats;
using GameDevLib.Units.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour, IMovable, IObserver<InputManagerArgs>
    {
        #region Links
        
        [field: Header( "Stats")] 
        [field: SerializeField] public MovingStats MovingStats { get; set; }

        [field: Header("Events")]
        [field: SerializeField]
        private InputEvent InputEvent { get; set; }
        
        #endregion
        
        #region Fields

        private Rigidbody _rigidbody;
        
        #endregion

        #region Properties 
        
        // Moving
        public float CurrentAccelerationFactor { get; set; }
        float IMovable.CurrentVelocity { get; set; }
        bool IMovable.IsSpeedUp { get; set; }
        bool IMovable.IsSpeedDown { get; set; }
        
        private InputManagerArgs? _args { get; set; }

        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            CurrentAccelerationFactor = MovingStats.AccelerationFactor;
        }

        private void OnEnable()
        {
            InputEvent.Attach(this);
        }

        private void OnDisable()
        {
            InputEvent.Detach(this);
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion
        
        #region Functionality

        public void Move()
        {
            // ... 
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            _args = args;
        }
        
        #endregion
    }
}