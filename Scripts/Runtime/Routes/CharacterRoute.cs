using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Enums;
using GameDevLib.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Routes
{
    public class CharacterRoute : MonoBehaviour
    {
        #region Links
        [SerializeField] private RouteStats stats;
        [SerializeField] private WayPoint[] wayPoints;
        #endregion
        
        #region Constants and variables
        private const int RouteStartIndex = 0;
        private int RouteEndIndex => wayPoints.Length - 1;
        //public Vector3 FirstWaypoint => wayPoints[RouteStartIndex].point.position;
        private float _spawnTimer = 0;
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
        
        #region Functionality
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
        /// Called when characters appear on route.
        /// </summary>
        /// <param name="item">A tuple of route number and number of spawned characters.</param>
        public void OnAppearanceCharacterEvent((int routeNumber, int createCount) item)
        {
            if (item.routeNumber != stats.RouteNumber) return;
            _countdownToSpawnNewEnemyCoroutine = StartCoroutine(CountdownToSpawnNewEnemyCoroutine());
        }
        
        /// <summary>
        /// Called when characters disappear on route.
        /// </summary>
        /// <param name="missing">A dictionary of route numbers and number of missing characters on each of them.</param>
        public void OnDisappearanceCharacterEvent(Dictionary<int, int> missing)
        {
            var killedOnCurrentRoute = missing.Select(el => el.Key == stats.RouteNumber).First();
            if (!killedOnCurrentRoute) return;
            _countdownToSpawnNewEnemyCoroutine = StartCoroutine(CountdownToSpawnNewEnemyCoroutine());
        }
        
        /// <summary>
        /// Called after appearance or disappearance of a character from route.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CountdownToSpawnNewEnemyCoroutine()
        {
            var minValue = stats.MinSpawnTimeout;
            var maxValue = stats.MinSpawnTimeout * 3;
            _spawnTimer = Random.Range(minValue, maxValue);
            
            yield return new WaitForSeconds(_spawnTimer);

            _spawnTimer = 0;
            _countdownToSpawnNewEnemyCoroutine = null;
        }
        #endregion
    }
}