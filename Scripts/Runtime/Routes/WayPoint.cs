using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Routes
{
    /// <summary>
    /// Represent Waypoint.
    /// </summary>
    [Serializable]
    public class WayPoint
    {
        #region Сonstants, variables & properties

        /// <summary>
        /// Point in space.
        /// </summary>
        public Transform point;
        
        /// <summary>
        /// Specifies that this is a checkpoint.
        /// </summary>
        public bool isCheckPoint;
        
        /// <summary>
        /// Specifies that character's attention is increased at this point.
        /// </summary>
        public bool isIncreaseAttention;

        #endregion
    }
}