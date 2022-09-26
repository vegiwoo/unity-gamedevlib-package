using System.Collections;
using UnityEngine;
using GameDevLib.Args;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using GameDevLib.Stats;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Weapon
{
    /// <summary>
    /// Represents the general weapon type in the game.
    /// </summary>
    [RequireComponent(typeof(AudioIsPlaying))]
    public class Weapon : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links
        [SerializeField] 
        public WeaponStats stats;
        
        [field:SerializeField, Tooltip("Firing point at end of gun barrel")] 
        public Transform ShotPoint { get; set; }
        
        [SerializeField] 
        private InputEvent inputEvent;
        
        [SerializeField] 
        private Light flashLight;
        
        [SerializeField] 
        private Light flashOnFire;

        [SerializeField] 
        public CharacterType targetCharacterType;
        #endregion
        
        #region Constants and variables 
        private string _targetTag;
        
        private AudioIsPlaying _audioIsPlaying;
        
        /// <summary>
        /// A timer counting down time until next shot is possible.
        /// </summary>
        private float _shotDelayTimer;
        
        private Coroutine _shotDelayCoroutine;
        private Coroutine _flashLightCoroutine;
        
        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            _audioIsPlaying = GetComponent<AudioIsPlaying>();
        }

        private void Start()
        {
            InitialFlashlightSetup();
            
            switch (targetCharacterType)
            {
                case CharacterType.Hero:
                    _targetTag = Data.PlayerTag ;
                    break;
                case CharacterType.Enemy:
                    _targetTag = Data.EnemyTag;
                    break;
            }

            _shotDelayTimer = 0f;
        }

        private void Update()
        {
            transform.localEulerAngles = new Vector3(-stats.TiltAngleInDeg, 0, 0);
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
        
        private void InitialFlashlightSetup()
        {
            if (!stats.IsHaveFlashlight) return;
            
            if (flashLight == null)
            {
                Debug.LogError("This weapon must be equipped with a flashlight!");
            }

            flashLight.intensity = stats.FlashlightIntensity;
            flashLight.range = stats.FlashlightRange;
            flashLight.spotAngle = stats.FlashlightAngle;

            flashLight.enabled = false;
        }
        
        /// <summary>
        /// Gets command to fire weapon.
        /// </summary>
        /// <param name="targetPosition">Target position to hit.</param>
        public void Fire(Vector3 targetPosition)
        {
            if (_shotDelayTimer > 0)
            {
                _audioIsPlaying.PlaySound(SoundType.Negative);
            }
            else
            {
                // TODO: Implement as object pool
                var newBullet = Instantiate(stats.BulletPrefab, ShotPoint.position, ShotPoint.rotation);
                var bulletSpeed = CalculateBulletSpeed(targetPosition);
                var args = new BulletArgs(_targetTag, ShotPoint, targetPosition, bulletSpeed, stats.ShotRange, stats.DamagePerShot);
                newBullet.Init(args);
            
                _audioIsPlaying.PlaySound(SoundType.Positive);
            
                _shotDelayCoroutine = StartCoroutine(ShotDelayCoroutine());
                _flashLightCoroutine = StartCoroutine(FlashOnFireCoroutine());
            }
        }
        
        /// <summary>
        /// Calculates speed of a bullet along a ballistic trajectory.
        /// </summary>
        /// <param name="targetPosition">Target position to hit.</param>
        /// <returns>Speed of bullet.</returns>
        /// <remarks>https://youtu.be/lXSzdGBIPkg</remarks>
        private float CalculateBulletSpeed(Vector3 targetPosition)
        {
            var fromShooterToTarget = targetPosition - transform.position;
            var fromShooterToTargetXZ = new Vector3(fromShooterToTarget.x, 0, fromShooterToTarget.z);
            transform.rotation = Quaternion.LookRotation(fromShooterToTargetXZ, Vector3.up);
            
            var x = fromShooterToTargetXZ.magnitude;
            var y = fromShooterToTarget.y;

            var angleInRadians = stats.TiltAngleInDeg * Mathf.PI / 180;
            var v2 = Data.Gravity * Mathf.Pow(x, 2) /
                     (2 * (y - Mathf.Tan(angleInRadians) * x) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));
            return Mathf.Sqrt(Mathf.Abs(v2));
        }

        private IEnumerator FlashOnFireCoroutine()
        {
            flashOnFire.enabled = true;
            yield return new WaitForSeconds(0.05f);
            flashOnFire.enabled = false;

            _flashLightCoroutine = null;
        }

        private IEnumerator ShotDelayCoroutine()
        {
            _shotDelayTimer = stats.ShotDelay;

            while (_shotDelayTimer > 0)
            {
                _shotDelayTimer -= Time.deltaTime;
                yield return null;
            }

            _shotDelayTimer = 0;
            _shotDelayCoroutine = null;
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if (stats.IsHaveFlashlight)
            {
                flashLight.enabled = args.Lighting;
            }
        }
        #endregion
    }
}