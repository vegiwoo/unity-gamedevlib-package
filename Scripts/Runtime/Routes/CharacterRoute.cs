using System.Collections;
using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using GameDevLib.Stats;
using JetBrains.Annotations;
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
        private RouteEvent routeEvent;
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
        /// Returns requested route transform.
        /// </summary>
        /// <param name="positionType">Requested item type.</param>
        /// <param name="i">Index of position relative to which result is requested.</param>
        /// <remarks>
        /// When receiving positions of type 'First' or 'Last', index is not specified.
        /// </remarks>>
        [CanBeNull] public Transform this[RoutePositionType positionType, int i]
        {
            get
            {
                return positionType switch
                {
                    RoutePositionType.Previous => wayPoints[i - 1].point,
                    RoutePositionType.Current => wayPoints[i].point,
                    RoutePositionType.Next => wayPoints[i + 1].point,
                    _ => null
                };
            }
        }
        
        #endregion
        
        #region MonoBehaviour methods

        private void OnEnable()
        {
            if (routeEvent != null)
            {
                routeEvent.Attach(this);
            }
        }
        
        private void OnDisable()
        {
            if (routeEvent != null)
            {
                routeEvent.Detach(this);
            }
        }

        #endregion
        
        #region Functionality

        public void Init(RouteEvent routeEvent)
        {
            this.routeEvent = routeEvent;
            this.routeEvent.Attach(this);
        }
        
        /// <summary>
        /// Change destination waypoint and direction of moving.
        /// </summary>
        /// <param name="isMovingForward">Current direction of moving.</param>
        /// <param name="oldIndex">Old index of waypoint .</param>
        /// <returns>Calculation result.</returns>
        public WaypointArgs ChangeWaypoint(bool isMovingForward, in int oldIndex)
        {
            const int step = 1;
            var isCurrentControlPoint = wayPoints[oldIndex].isCheckPoint;
            var isAttentionIsIncreased = wayPoints[oldIndex].isIncreaseAttention;
            int newIndex;
            bool isNewMovingForward = default;
            
            switch (isMovingForward)
            {
                case true:
                    if (oldIndex != RouteEndIndex)
                    {
                        newIndex = oldIndex + step;
                        isNewMovingForward = true;
                    }
                    else
                    {
                        newIndex = stats.IsLoopedRoute ? RouteStartIndex : oldIndex - step;
                    }
                    break;
                case false:
                    if (oldIndex != RouteStartIndex)
                    {
                        newIndex = oldIndex - step;
                    }
                    else
                    {
                        newIndex = oldIndex + step;
                        isNewMovingForward = true;
                    }
                    break;
            }

            return new WaypointArgs(newIndex, wayPoints[newIndex].point, isNewMovingForward, isCurrentControlPoint,
                isAttentionIsIncreased);
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

        public void OnEventRaised(ISubject<RouteArgs> subject, RouteArgs args)
        {
            if(args.RouteName != stats.RouteName) return;
            
            if (args.NumberCharactersCreated > 0 || args.NumberCharactersMissing > 0)
            {
                _countdownToSpawnNewEnemyCoroutine = StartCoroutine(CountdownToSpawnNewEnemyCoroutine());
            }
        }
        #endregion
    }
}