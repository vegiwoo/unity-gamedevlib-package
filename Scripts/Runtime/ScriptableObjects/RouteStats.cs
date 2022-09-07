using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "RouteStats", menuName = "Stats/RouteStats", order = 3)]
    public class RouteStats : ScriptableObject
    {
        [field: SerializeField, Tooltip("Route number")] 
        public int RouteNumber { get; set; }

        [field: SerializeField, Tooltip("Looped route")]
        public bool IsLoopedRoute { get; set; }

        [field: SerializeField, ReadonlyField, Tooltip("Minimum timeout before appearance of a new character on route")] 
        public float MinSpawnTimeout { get; set; } = Random.Range(2, 4);

        [field:SerializeField, Tooltip("Maximum number of enemies on the route")] 
        public int MaxNumberEnemies { get; set; }

        [field: SerializeField, Tooltip("Attention trigger size increase factor on checkpoints")]
        public float AttentionIncreaseFactor { get; set; } = 1.5f;
    }
}