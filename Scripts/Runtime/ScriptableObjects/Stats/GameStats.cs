using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "GameStats", menuName = "Stats/GameStats", order = 0)]
    public class GameStats : ScriptableObject
    {
        [field: Header("Common")]
        [field: SerializeField] public int GameHighScore { get; set; } = 100;
        
        [field: Header("Units")]
        [field: SerializeField, Range(10, 50), Tooltip("Threshold at which values (game points, hp) are considered low")]
        public float CriticalThreshold { get; set; } = 15;
        
        [field: Header("Effects")]
        [field: SerializeField, Range(5f, 20.0f)] public float EffectDuration { get; set; } = 10.0f;
        
    }
}