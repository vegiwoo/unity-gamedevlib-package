using System;
using System.Collections;
using System.Linq;
using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Helpers;
using GameDevLib.Interactions;
using GameDevLib.Interfaces;
using GameDevLib.Routes;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    /// <summary>
    /// Represents item of an enemy.
    /// </summary>
    /// <remarks>
    /// https://docs.unity3d.com/540/Documentation/Manual/nav-CouplingAnimationAndNavigation.html
    /// </remarks>>
    [RequireComponent( typeof(Rigidbody),typeof(NavMeshAgent),typeof(Character) )]
    public class CharacterMovement : MonoBehaviour, IAnimatorParametersWorkable
    {
        #region Links
        [SerializeField] 
        private CharacterRoute route;
        
        [field: SerializeField, Tooltip("Current state of enemy"), ReadonlyField] 
        public CharacterState CurrentState { get; private set; }

        private Rigidbody _rb;
        private NavMeshAgent _navMeshAgent;
        private Character _character;
        private TrackedTrigger _trackedTrigger;

        private Coroutine _characterPatrolCoroutine;
        private Coroutine _characterAttackCoroutine;
        
        private Transform _aimingPoint;
        
        /// <summary>
        /// Character wait timer at checkpoint.
        /// </summary>
        private float currentCountdownValue;
        #endregion
        
        #region Constants and variables
        /// <summary>
        /// Index of current waypoint.
        /// </summary>
        private int _currentWaypointIndex;
        
        /// <summary>
        /// Flag of enemy's movement forward along route.
        /// </summary>
        private bool _isMovingForward;
        
        // keys for animator
        private int _isWalking;
        private int _velocityX;
        private int _velocityY;
        #endregion

        #region MonoBehaviour methods


        #endregion

        #region Functionality

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _character = GetComponent<Character>();
            _trackedTrigger = GetComponentInChildren<TrackedTrigger>();
        }

        private void Start()
        {
            _rb.isKinematic = true;

            if(_character.Stats.characterType == CharacterType.Enemy)
            {
                gameObject.tag = Data.EnemyTag;
            }

            _character.CurrentSpeed = _character.Stats.MoveSpeed;
            
            _navMeshAgent.speed = _character.CurrentSpeed;
            _navMeshAgent.stoppingDistance = _character.Stats.StopDistanceForWaypoints;
            _navMeshAgent.updatePosition = false;
            
            _isMovingForward = true;
            _currentWaypointIndex = 0;
            
            _trackedTrigger.Init(_character.Stats.TrackedObjectTypes);
            
            currentCountdownValue = 0;
            
            ToggleEnemyState(CharacterState.Patrol);
        }
        #endregion
        
        #region Functionality
        /// <summary>
        /// Changes state of character.
        /// </summary>
        /// <param name="state">New state of character.</param>
        private void ToggleEnemyState(CharacterState state)
        {
            CurrentState = state;

            switch (CurrentState)
            {
                case CharacterState.Patrol:
                    _aimingPoint = null;
                    _characterPatrolCoroutine = StartCoroutine(CharacterPatrolCoroutine());
                    _trackedTrigger.TrackedTriggerNotify += TrackedTriggerHandler;
                    break;
                case CharacterState.Attack:
                    //_enemyAttackCoroutine = StartCoroutine(EnemyAttackCoroutine());
                    _trackedTrigger.TrackedTriggerNotify -= TrackedTriggerHandler;
                    break;
            }
        }
        
        /// <summary>
        /// Moves character when attacking.
        /// </summary>
        private void TrackedTriggerHandler(TrackedTriggerArgs args)
        {
            if (!_character.Stats.TrackedObjectTypes.Contains(args.TrackedObjectType)) return;

            switch (args.TrackedObjectType)
            {
                case TrackedObjectType.Hero:
                    _aimingPoint = args.TrackedObjectTransform;
                    ToggleEnemyState(CharacterState.Attack);
                    break;
            }
        }

        private IEnumerator CharacterPatrolCoroutine()
        {
            // Dummy
            yield return null;
        }
        

        // ...
        public void AssignAnimationIDs()
        {
            _isWalking = Animator.StringToHash("isWalking");
            _velocityX = Animator.StringToHash("velX");
            _velocityY = Animator.StringToHash("velY");
        }
        #endregion
        

    }
}



