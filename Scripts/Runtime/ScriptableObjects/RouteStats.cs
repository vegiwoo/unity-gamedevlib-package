using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "RouteStats", menuName = "Stats/RouteStats", order = 3)]
    public class RouteStats : ScriptableObject
    {
        [field: SerializeField, Tooltip("Route number")] 
        public string RouteName { get; set; }

        [field: SerializeField, Tooltip("Looped route")]
        public bool IsLoopedRoute { get; set; }

        [field: SerializeField, ReadonlyField, Tooltip("Minimum timeout before appearance of a new character on route")] 
        public int MinSpawnTimeout { get; set; } = 4;

        [field: SerializeField, Tooltip("Maximum number of enemies on route")]
        public int MaхNumberCharacters { get; set; } = 1;

        [field: SerializeField, Tooltip("Attention trigger size increase factor on checkpoints")]
        public float AttentionIncreaseFactor { get; set; } = 1.5f;

        [field: SerializeField, Tooltip("Waiting time at checkpoints"), Range(1, 10)]
        public float WaitTime { get; set; } = 3.0f;
    }
}