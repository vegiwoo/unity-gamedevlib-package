using System;
using System.Collections;
using System.Linq;
using GameDevLib.Animations;
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
    [RequireComponent(typeof(Animator),typeof(NavMeshAgent),typeof(Character) )]
    public class CharacterMovement : MonoBehaviour, IAnimatorParametersWorkable
    {
        #region Links
        [field:SerializeField, ReadonlyField] 
        public CharacterRoute Route { get; set; }
        
        [field: SerializeField, Tooltip("Current state of enemy"), ReadonlyField] 
        public CharacterState CurrentState { get; private set; }

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Character _character;
        private TrackedTrigger _trackedTrigger;
        private IKControl _ikControl;

        private Coroutine _characterPatrolCoroutine;

        private Transform _pointOfInterest;
        
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
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _character = GetComponent<Character>();
            _animator = GetComponent<Animator>();
            _trackedTrigger = GetComponentInChildren<TrackedTrigger>();
            _ikControl = GetComponent<IKControl>();
        }

        private void Start()
        {
            if(_character.Stats.characterType == CharacterType.Enemy)
            {
                gameObject.tag = Data.EnemyTag;
            }

            _character.CurrentSpeed = _character.Stats.MoveSpeed;
            
            _navMeshAgent.speed = _character.CurrentSpeed;
            _navMeshAgent.stoppingDistance = _character.Stats.StopDistanceForWaypoints;
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            
            _isMovingForward = true;
            _currentWaypointIndex = 0;
            
            _trackedTrigger.Init(_character.Stats.TrackedObjectTypes);
            
            currentCountdownValue = 0;

            _ikControl.HeadTrackingDistance = _character.Stats.MaxDistance;

            AssignAnimationIDs();
            
            ToggleCharacterState(CharacterState.Patrol);
        }

        private void Update()
        {
            if (CurrentState != CharacterState.Died)
            {
                OnMovement();
                DirectCharacterToPointOfInterest();
            }
        }

        private void OnEnable()
        {
            _trackedTrigger.TrackedTriggerNotify += TrackedTriggerHandler;
            _character.CharacterHandlerNotify += OnCharacterEventHandler;
        }
        
        private void OnDisable()
        {
            _trackedTrigger.TrackedTriggerNotify -= TrackedTriggerHandler;
            _character.CharacterHandlerNotify -= OnCharacterEventHandler;
        }

        private void OnAnimatorMove()
        {
            transform.position = _navMeshAgent.nextPosition;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Functionality

        private void OnMovement()
        {
            // Moving
            var t = gameObject.transform;
            var worldDeltaPosition = _navMeshAgent.nextPosition - t.position;
            var groundDeltaPosition = Vector3.zero;
            groundDeltaPosition.x = Vector3.Dot(t.right, worldDeltaPosition);
            groundDeltaPosition.y = Vector3.Dot(t.forward, worldDeltaPosition);
            var velocity = (Time.deltaTime > 1e-5f) ? groundDeltaPosition / Time.deltaTime : Vector3.zero;
            var shouldMove = velocity.magnitude > 0.025f && _navMeshAgent.remainingDistance > _navMeshAgent.radius;
            
            _animator.SetBool(_isWalking, shouldMove);
            _animator.SetFloat (_velocityX, velocity.x);
            _animator.SetFloat (_velocityY, velocity.y);

            // Rotate
            var currentPoint = Route[RoutePositionType.Current, _currentWaypointIndex];
            if (currentPoint is not null)
            {
                var direction = currentPoint.position - transform.position;
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));  
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _character.Stats.RotationSmoothTime);;
            }
            
            // Fix
            if (worldDeltaPosition.magnitude > _navMeshAgent.radius)
            {
                transform.position = _navMeshAgent.nextPosition - 0.9f * worldDeltaPosition;
            }
        }

        /// <summary>
        /// Changes state of character.
        /// </summary>
        /// <param name="state">New state of character.</param>
        private void ToggleCharacterState(CharacterState state)
        {
            CurrentState = state;

            switch (CurrentState)
            {
                case CharacterState.Patrol:
                    if (Route != null)
                    {
                        _pointOfInterest = null;
                        _characterPatrolCoroutine = StartCoroutine(CharacterPatrolCoroutine());
                        _trackedTrigger.TrackedTriggerNotify += TrackedTriggerHandler;
                    }
                    else
                    {
                        Debug.LogError("An attempt to move a character without specifying a route!");
                    }
                    break;
                case CharacterState.Attack:
                    //_enemyAttackCoroutine = StartCoroutine(EnemyAttackCoroutine());
                    _trackedTrigger.TrackedTriggerNotify -= TrackedTriggerHandler;
                    break;
                case CharacterState.Died:
                    
                    StopAllCoroutines();
                    
                    _pointOfInterest = null;
                    _character.CurrentSpeed =  _navMeshAgent.speed = 0;
                    
                    _trackedTrigger.TrackedTriggerNotify -= TrackedTriggerHandler;
                    _character.CharacterHandlerNotify -= OnCharacterEventHandler;
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
                    _pointOfInterest = args.TrackedObjectTransform;
                    ToggleCharacterState(CharacterState.Attack);
                    break;
            }
        }

        private void OnCharacterEventHandler(CharacterArgs args)
        {
            if (args.DiedByTimer)
            {
               ToggleCharacterState(CharacterState.Died);
            }
        }

        private (float distance, bool isCanGetCloser)? GetDistanceToPointOfInterest()
        {
            if (_pointOfInterest == null) return null;

            var distanceToPointOfInterest = Vector3.Distance(transform.position, _pointOfInterest.position);
            var isCanGetCloser = distanceToPointOfInterest > _character.Stats.MinDistance &&
                                 distanceToPointOfInterest < _character.Stats.MaxDistance;
            return (distanceToPointOfInterest, isCanGetCloser);
        }
        
        private void DirectCharacterToPointOfInterest()
        {
            if(gameObject == null || 
               !_navMeshAgent.isActiveAndEnabled || 
               CurrentState != CharacterState.Attack || 
               _pointOfInterest == null) return;

            var dist = GetDistanceToPointOfInterest();
            if(dist is not { isCanGetCloser: true }) return;

            _navMeshAgent.ResetPath(); 
            _navMeshAgent.destination = _pointOfInterest.position;

            _ikControl.TargetForHead = _pointOfInterest;
        }

        private IEnumerator CharacterPatrolCoroutine()
        {
            while (CurrentState == CharacterState.Patrol && _pointOfInterest == null)
            {
                var currentWaypoint = Route[RoutePositionType.Current, _currentWaypointIndex];

                if (_character.CurrentHp > 0 && _navMeshAgent.isActiveAndEnabled)
                {
                    if (currentWaypoint != null)
                    {
                        _navMeshAgent.destination = currentWaypoint.position;
                    }
                }

                var stopDistance = _navMeshAgent.stoppingDistance;

                // Change waypoint
                if (currentWaypoint is not null && 
                    Math.Abs(transform.position.x - currentWaypoint.position.x) < stopDistance &&
                    Math.Abs(transform.position.z - currentWaypoint.position.z) < stopDistance)
                {
                    var result = Route.ChangeWaypoint(_isMovingForward, _currentWaypointIndex);
                    
                    // Waiting if point is checkpoint
                    if (result.IsControlPoint)
                    {
                        yield return StartCoroutine(WaitingCoroutine(result.IsAttentionIsIncreased,
                            Route.stats.WaitTime));
                    }

                    _isMovingForward = result.IsMoveForward;
                    _currentWaypointIndex = result.NewCurrentIndex;
                }

                if (CurrentState == CharacterState.Patrol)
                {
                    yield return null;
                }
                else
                {
                    Debug.Log("CharacterPatrolCoroutine exit");
                    _characterPatrolCoroutine = null;
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// Coroutine chasing and attacking enemy.
        /// </summary>
        private IEnumerator CharacterAttackCoroutine()
        {
            while (CurrentState == CharacterState.Attack && _pointOfInterest != null)
            {
                var dist = GetDistanceToPointOfInterest();

                if (dist is { isCanGetCloser: true } && gameObject != null && _navMeshAgent.isActiveAndEnabled)
                {
                    if (dist.Value.isCanGetCloser)
                    {
                        _navMeshAgent.ResetPath();
                        _navMeshAgent.destination = _pointOfInterest.position;
                    }
                    yield return null;
                }
                else
                {
                    _navMeshAgent.ResetPath();
                    yield return new WaitForSeconds(Route.stats.WaitTime);
                    
                    ToggleCharacterState(CharacterState.Patrol);
                    //_characterAttackCoroutine = null;
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// Coroutine waiting for enemy at point.
        /// </summary>
        /// <param name="changeSizeOfDiscoveryTrigger">Need to change size of discovery trigger.</param>
        /// <param name="countdownValue">Wait timer value.</param>
        /// <returns></returns>
        private IEnumerator WaitingCoroutine(bool changeSizeOfDiscoveryTrigger, float countdownValue = 5)
        {
            currentCountdownValue = countdownValue;
            var savedSpeed = _navMeshAgent.velocity;

            if (changeSizeOfDiscoveryTrigger)
            {
                _trackedTrigger.ChangeSizeOfDiscoveryTrigger(true, Route.stats.AttentionIncreaseFactor);
            }

            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.isStopped = true;

            while (currentCountdownValue > 0)
            {
                yield return new WaitForSeconds(1.0f);
                currentCountdownValue--;
            }

            if (changeSizeOfDiscoveryTrigger)
            {
                _trackedTrigger.ChangeSizeOfDiscoveryTrigger(false);
            }

            currentCountdownValue = 0;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.velocity = savedSpeed;

            yield return null;
        }

        public void AssignAnimationIDs()
        {
            _isWalking = Animator.StringToHash("isWalking");
            _velocityX = Animator.StringToHash("velX");
            _velocityY = Animator.StringToHash("velY");
        }
        #endregion
    }
}