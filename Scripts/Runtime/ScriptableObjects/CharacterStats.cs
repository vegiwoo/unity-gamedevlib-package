using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Stats/CharacterStats", order = 1)]
    public class CharacterStats : ScriptableObject
    {
        [field: SerializeField] 
        public float MaxHp { get; set; }
        
        [field: SerializeField, Range(1f,5f)]
        public float BaseMovingSpeed { get; set; }
        
        [field: SerializeField, Range(1f,5f)]
        public float BaseRotationSpeed { get; set; }
        
        [field: SerializeField, Range(1.5f, 5.0f), Tooltip("Value by which base speed is multiplied when running")]
        public float AccelerationFactor { get; set; }
        
        [field: SerializeField, Range(0.5f, 1.0f)]
        public float JumpHeight { get; set; }
    }
}