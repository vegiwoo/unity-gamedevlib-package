using GameDevLib.Enums;
using GameDevLib.Ammunition;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Stats
{
    [CreateAssetMenu(fileName = "WeaponStats", menuName = "Stats/WeaponStats", order = 2)]
    public class WeaponStats : ScriptableObject
    { 
        [field: Header("Common")]
        [field: SerializeField]
        public WeaponType WeaponType { get; set; }
        [field:SerializeField] 
        public Bullet BulletPrefab { get; set; }
        
        [field:Header("Weapon stats")]
        [field:SerializeField, Tooltip("Shot range in meters"), Range(20f,50f)]
        public float ShotRange { get; set; }
        
        [field:SerializeField, Tooltip("Shot delay in seconds"), Range(0.1f, 5.0f)]
        public float ShotDelay { get; set; }
        
        [field:SerializeField, Tooltip("Weapon tilt angle in degrees"), Range(0f, 45f)]
        public int TiltAngleInDeg { get; set; }
        
        [field:SerializeField, Tooltip("Damage per shot"), Range(5, 15)]
        public int DamagePerShot { get; set; }
        
        [field: Header("Optional equipment")]
        [field: SerializeField, Tooltip("Weapon is equipped with a flashlight")]
        public bool IsHaveFlashlight { get; set; }
        [field: SerializeField, Range(3f,10f)]
        public float FlashlightIntensity { get; set; }
        [field: SerializeField, Range(10f,30f)]
        public float FlashlightRange { get; set; }
        [field: SerializeField, Range(30f,50f)]
        public float FlashlightAngle { get; set; }
    }
}
