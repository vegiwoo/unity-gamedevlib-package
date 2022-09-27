using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "GameStats", menuName = "GameBevLib/Stats/GameStats", order = 0)]
    public class MovingStats : ScriptableObject
    {
         #region Properties
         
        [field: Header("Movement")]
        [field: SerializeField, Range(1f, 5f)] public float MoveSpeed { get; set; } = 2.0f;

        [field: SerializeField, Range(1.5f, 5.0f), Tooltip("Value by which base speed is multiplied when running")]
        public float AccelerationFactor { get; set; } = 2.6f;

        [field: SerializeField, Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate { get; set; } = 10.0f;

        [field: SerializeField, Range(1.0f, 5.0f)]
        public float BaseRotationSpeed { get; set; } = 4.0f;

        [field: SerializeField, Tooltip("How fast the character turns to face movement direction. "), Range(0.0f, 5.0f)]
        public float RotationSmoothTime { get; set; } = 0.12f;

        [field: Space(10)] 
        [field: SerializeField, Range(0.5f, 2.0f)]
        public float JumpHeight { get; set; } = 1.2f;

        [field: SerializeField, Tooltip("The character uses its own gravity value. The engine default is -9.81f"), Range(-10, -20)]
        public float Gravity { get; set; }  = -15.0f;

        [field:SerializeField, Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout { get; set; } = 0.50f;

        [field: Space(10)]
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout { get; set; } = 0.15f;

        [field:Header("Player Grounded")]
        [field: SerializeField, Tooltip("Useful for rough ground")] 
        public float GroundedOffset { get; set; }= -0.14f;

        [field: SerializeField, Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius { get; set; } = 0.28f;

        [field:Header("For other characters")]
        [field: SerializeField, ReadonlyField]
        public float RotationAngleDelta { get; set; } = 7;

        [field: SerializeField, ReadonlyField] 
        public float MovingDistanceDelta { get; set; } = 0.02f;
        
        // [field: SerializeField, Tooltip("Contact distance with waypoint."), Range(0.1f, 1.0f)]
        // public float StopDistanceForWaypoints { get; set; } = 0.35f;
        //
        // [field: SerializeField, Tooltip("Minimum distance to point of interest."), Range(3, 10)]
        // public float MinDistance { get; set; } = 5.0f;
        //
        // [field: SerializeField, Tooltip("Maximum distance to point of interest."), Range(3, 10)]
        // public float MaxDistance { get; set; } = 15.0f;
        //
        // [field: SerializeField, Tooltip("Array of tracked types for characters")]
        // public TrackedObjectType[] TrackedObjectTypes { get; set; }
        
        #endregion
    }
}