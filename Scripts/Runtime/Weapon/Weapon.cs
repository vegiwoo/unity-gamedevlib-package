using UnityEngine;
using GameDevLib.Args;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Stats;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Weapon
{
    public class Weapon : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links
        [SerializeField] 
        private WeaponStats stats;
        
        [field:SerializeField, Tooltip("Firing point at end of gun barrel")] 
        public Transform ShotPoint { get; set; }
        
        [SerializeField] 
        private InputManagerEvent inputEvent;
        
        [SerializeField] 
        private Light flashLight;
        
        [SerializeField, Range(1.0f,5.0f)] 
        private float flashLightIntensity;
        
        [SerializeField] 
        public CharacterType targetCharacterType;
        #endregion
        
        #region Constants and variables 
        private string _targetTag;
        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            if (stats.IsHaveFlashlight && flashLight == null)
            {
                Debug.LogError("This weapon must be equipped with a flashlight!");
            }
            flashLight.intensity = flashLightIntensity;
            flashLight.enabled = false;
        }
        
        private void OnEnable()
        {
            inputEvent.Attach(this);
        }

        private void OnDisable()
        {
            inputEvent.Detach(this);
        }

        #endregion

        #region Functionality
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if (args.Lighting != null && stats.IsHaveFlashlight)
            {
                flashLight.enabled = args.Lighting.Value;
            }
        }
        #endregion
    }
}