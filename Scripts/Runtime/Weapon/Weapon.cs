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

        [SerializeField] 
        public CharacterType targetCharacterType;
        #endregion
        
        #region Constants and variables 
        private string _targetTag;
        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            if (stats.IsHaveFlashlight)
            {
                if (flashLight == null)
                {
                    Debug.LogError("This weapon must be equipped with a flashlight!");
                }

                flashLight.intensity = stats.FlashlightIntensity;
                flashLight.range = stats.FlashlightRange;
                flashLight.spotAngle = stats.FlashlightAngle;
                
                flashLight.enabled = false;
            }
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
                flashLight.enabled = !flashLight.enabled;
            }
        }
        #endregion
    }
}