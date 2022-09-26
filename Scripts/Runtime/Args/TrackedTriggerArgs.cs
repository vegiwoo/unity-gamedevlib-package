using GameDevLib.Enums;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public class TrackedTriggerArgs
    {
        public TrackedObjectType TrackedObjectType { get; } 
        public Transform TrackedObjectTransform { get; } 
        /// <summary>
        /// A flag that determines whether object enters or exits trigger..
        /// </summary>
        public bool IsTrackedObjectEnters { get; }

        public TrackedTriggerArgs(TrackedObjectType trackedObjectType, Transform trackedObjectTransform,
            bool isTrackedObjectEnters)
        {
            TrackedObjectType = trackedObjectType;
            TrackedObjectTransform = trackedObjectTransform;
            IsTrackedObjectEnters = isTrackedObjectEnters;

        }
    }
}