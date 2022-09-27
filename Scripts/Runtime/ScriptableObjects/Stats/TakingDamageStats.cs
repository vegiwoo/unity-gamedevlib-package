using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    public class TakingDamageStats : ScriptableObject
    {
        [field: SerializeField] public float MaxHp { get; set; }
    }
}