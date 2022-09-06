using GameDevLib.Enums;
using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Stats/CharacterStats", order = 1)]
    public class CharacterStats : ScriptableObject
    {
        #region Properties
        [field: SerializeField] public CharacterType type;

        [field: SerializeField] public float MaxHp { get; set; }

        [field: Header("Movement")]
        [field: SerializeField, Range(1f, 5f)]
        public float MoveSpeed { get; set; } = 2.0f;

        [field: SerializeField, Range(1.5f, 5.0f), Tooltip("Value by which base speed is multiplied when running")]
        public float AccelerationFactor { get; set; } = 2.6f;

        [field: SerializeField, Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate { get; set; } = 10.0f;

        [field: SerializeField, Range(1.0f, 5.0f)]
        public float BaseRotationSpeed { get; set; }

        [field: SerializeField, Tooltip("How fast the character turns to face movement direction"), Range(0.0f, 0.3f)]
        public float RotationSmoothTime { get; set; } = 0.12f;


        [field: Space(10)] 
        [field: SerializeField, Range(0.5f, 2.0f)]
        public float JumpHeight { get; set; } = 1.2f;

        [field: SerializeField, ReadonlyField, Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity { get; set; }  = -15.0f;

        [field:SerializeField, Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout { get; set; } = 0.50f;

        [field: Space(10)]
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout { get; set; } = 0.15f;

        [field:Header("Player Grounded")]
        [field: SerializeField, ReadonlyField, Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded { get; set; } = true;

        [field: SerializeField, Tooltip("Useful for rough ground")] 
        public float GroundedOffset { get; set; }= -0.14f;

        [field: SerializeField, Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius { get; set; } = 0.28f;

        [field: SerializeField, Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers { get; set; }
        #endregion
    }
}