using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public struct WaypointArgs
    {
        public int NewCurrentIndex { get; }
        public Transform NewPoint { get; }
        public bool IsMoveForward { get; }
        public bool IsControlPoint { get; }
        public bool IsAttentionIsIncreased { get; }

        public WaypointArgs(int newCurrentIndex, Transform newPoint, bool isMoveForward, bool isControlPoint, bool isAttentionIsIncreased)
        {
            NewCurrentIndex = newCurrentIndex;
            NewPoint = newPoint;
            IsMoveForward = isMoveForward;
            IsControlPoint = isControlPoint;
            IsAttentionIsIncreased = isAttentionIsIncreased;
        }
    }
}