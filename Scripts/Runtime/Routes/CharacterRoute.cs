using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using GameDevLib.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Routes
{
    public class CharacterRoute : MonoBehaviour, Interfaces.IObserver<RouteArgs>
    {
        #region Links
        [SerializeField] public RouteStats stats;
        [SerializeField] private WayPoint[] wayPoints; 
        private CharacterManagerEvent _characterManagerEvent;
        #endregion
        
        #region Constants and variables
        private const int RouteStartIndex = 0;
        private int RouteEndIndex => wayPoints.Length - 1;
        public Vector3 FirstWaypoint => wayPoints[RouteStartIndex].point.position;
        public float SpawnTimer { get; private set; } = 0;

        private Coroutine _countdownToSpawnNewEnemyCoroutine;
        #endregion
        
        #region Properties
        /// <summary>
        /// Returns requested route positions.
        /// </summary>
        /// <param name="positionType">Requested item type.</param>
        /// <param name="i">Index of position relative to which result is requested.</param>
        /// <remarks>
        /// When receiving positions of type 'First' or 'Last', index is not specified.
        /// </remarks>>
        public Vector3 this[RoutePositionType positionType, int i]
        {
            get
            {
                return positionType switch
                {
                    RoutePositionType.Previous => wayPoints[i - 1].point.position,
                    RoutePositionType.Current => wayPoints[i].point.position,
                    RoutePositionType.Next => wayPoints[i + 1].point.position,
                    _ => Vector3.zero
                };
            }
        }
        #endregion
        
        #region MonoBehaviour methods

        private void OnEnable()
        {
            if (_characterManagerEvent != null)
            {
                _characterManagerEvent.Attach(this);
            }
        }
        
        private void OnDisable()
        {
            if (_characterManagerEvent != null)
            {
                _characterManagerEvent.Detach(this);
            }
        }

        #endregion
        
        #region Functionality

        public void Init(CharacterManagerEvent characterManagerEvent)
        {
            _characterManagerEvent = characterManagerEvent;
            _characterManagerEvent.Attach(this);
        }
        
        /// <summary>
        /// Change destination waypoint and direction of moving.
        /// </summary>
        /// <param name="isMovingForward">Current direction of moving.</param>
        /// <param name="oldIndex">Old index of waypoint .</param>
        /// <returns>Calculation result.</returns>
        public (bool isMoveForward, int index, bool isControlPoint, bool isAttentionIsIncreased) ChangeWaypoint(bool isMovingForward, in int oldIndex)
        {
            const int step = 1;
            var isCurrentControlPoint = wayPoints[oldIndex].isCheckPoint;
            var isAttentionIsIncreased = wayPoints[oldIndex].isIncreaseAttention;

            return isMovingForward switch
            {
                true => oldIndex != RouteEndIndex
                    ? (true, oldIndex + step, isCurrentControlPoint, isAttentionIsIncreased)
                    : (false, stats.IsLoopedRoute ? RouteStartIndex : oldIndex - step, isCurrentControlPoint, isAttentionIsIncreased),
                false => oldIndex != RouteStartIndex
                    ? (false, oldIndex - step, isCurrentControlPoint, isAttentionIsIncreased)
                    : (true, oldIndex + step, isCurrentControlPoint, isAttentionIsIncreased)
            };
        }

        /// <summary>
        /// Called after appearance or disappearance of a character from route.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CountdownToSpawnNewEnemyCoroutine()
        {
            var minValue = stats.MinSpawnTimeout;
            var maxValue = stats.MinSpawnTimeout * 3;
            SpawnTimer = Random.Range(minValue, maxValue);
            
            yield return new WaitForSeconds(SpawnTimer);

            SpawnTimer = 0;
            _countdownToSpawnNewEnemyCoroutine = null;
        }
        #endregion

        public void OnEventRaised(ISubject<RouteArgs> subject, RouteArgs args)
        {
            if(args.RouteName != stats.RouteName) return;
            
            if (args.NumberCharactersCreated > 0 || args.NumberCharactersMissing > 0)
            {
                _countdownToSpawnNewEnemyCoroutine = StartCoroutine(CountdownToSpawnNewEnemyCoroutine());
            }
        }
    }
}