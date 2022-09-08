using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Helpers;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Interactions
{
    public class TrackedTrigger : MonoBehaviour
    {
        #region Links
        
        [field:SerializeField]
        public TrackedObjectType[] TrackedObjectTypes { get; private set;}
        
        #endregion
        
        #region Constants and variables
        
        private Vector3 originalSize;
        
        // Events
        public delegate void TrackedTriggerHandler(TrackedTriggerArgs args);  
        [CanBeNull]  public event TrackedTriggerHandler? TrackedTriggerNotify;
        
        #endregion
        
        #region MonoBehaviour methods 
        
        private void Start()
        {
            originalSize = transform.localScale;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            HandlingTriggerEvent(other.gameObject, true);
        }

        private void OnTriggerExit(Collider other)
        {
            HandlingTriggerEvent(other.gameObject, false);
        }
        
        #endregion
        
        #region Functionality
        public void Init(TrackedObjectType[] trackedObjectTypes)
        {
            TrackedObjectTypes = trackedObjectTypes;
        }

        /// <summary>
        /// Checks if game object in trigger matches one of discovery types.
        /// </summary>
        /// <param name="obj">Game object that hit trigger.</param>
        /// <returns>Match one of discovery types, or null.</returns>
        [CanBeNull]
        private static TrackedObjectType? GetTrackedObjectTypeFromObject(in GameObject obj)
        {
            if (obj.CompareTag(Data.PlayerTag))
            {
                return TrackedObjectType.Hero;
            } 
            
            if (obj.CompareTag(Data.EnemyTag))
            {
                return TrackedObjectType.Enemy;
            } 
            
            if(obj.TryGetComponent<MoveableObject>(out var movable))
            {
                return TrackedObjectType.Movable;
            }
            
            return null;
        }
        
        /// <summary>
        /// Handles an event when an object enters and exits a trigger.
        /// </summary>
        /// <param name="obj">Game object that hit trigger.</param>
        /// <param name="isObjectEnters">An object enters or exits a trigger.</param>
        private void HandlingTriggerEvent(in GameObject obj, in bool isObjectEnters)
        {
            if (TrackedObjectTypes is null || TrackedObjectTypes.Length == 0) return;
   
            var type = GetTrackedObjectTypeFromObject(obj);
            if (type == null) return;

            var args = new TrackedTriggerArgs((TrackedObjectType)type, obj.transform, isObjectEnters);
            TrackedTriggerNotify?.Invoke(args);
        }

        /// <summary>
        /// Changes physical size of trigger.
        /// </summary>
        /// <param name="increase">Trigger increment/decrement flag.</param>
        /// <param name="factor">Magnification multiplier.</param>
        public void ChangeSizeOfDiscoveryTrigger(bool increase, float? factor = null)
        {
            var currentScale = transform.localScale;
            transform.localScale = increase && factor != null ? 
                (Vector3)(currentScale * factor) :
                originalSize;;
        }
        
        #endregion
    }
}

