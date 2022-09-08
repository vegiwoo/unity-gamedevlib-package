using System.Collections;
using GameDevLib.Animations;
using GameDevLib.Helpers;
using GameDevLib.Stats;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    /// <summary>
    /// Essence of playable or non-playable character.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(IKControl))]
    public abstract class Character : MonoBehaviour
    {
        #region Links
        [field: Header("Links")]
        [field: SerializeField, Tooltip("Physical bodies in a ragdoll object")] 
        private Rigidbody[] rigidBodiesForRagdoll;

        [field:SerializeField] 
        public CharacterStats Stats { get; private set; }
        [field: SerializeField, ReadonlyField]
        
        public float CurrentHp { get; protected set; }
        [field: SerializeField, ReadonlyField]
        
        public float CurrentSpeed { get; set; }

        [field: SerializeField, Tooltip("What layers the character uses as ground")]
        public LayerMask[] GroundLayers { get; set; }
        
        [field: Header("Kill by timer")]
        [field: SerializeField] 
        public bool IsDeathByTimer { get; set; } = false;
        [field: SerializeField, Tooltip("in sec")]
        public float DeathTimerValue { get; set; } = 3f;

        [field: SerializeField, Tooltip("Delay before destruction after death by timer")] 
        private float DelayDeathByTimer { get; set; } = 5f;

        private Coroutine _killOnTimerCoroutine;

        public Animator Animator { get; private set; }
        //public IKControl IKControl { get; private set; }
        #endregion
        
        #region Constants and variables 
        
        public delegate void CharacterHandler(CharacterArgs args);  
        [CanBeNull]  public event CharacterHandler? CharacterHandlerNotify;
        #endregion

        #region MonoBehaviour methods

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
            //IKControl = GetComponent<IKControl>();
        }
        
        protected void Start()
        {
            ToggleRagdoll(false);
            
            CurrentHp = Stats.MaxHp;
            CurrentSpeed = Stats.MoveSpeed;
        }

        protected void Update()
        {
            if (IsDeathByTimer && _killOnTimerCoroutine == null)
            {
                _killOnTimerCoroutine = StartCoroutine(KillOnTimerCoroutine());
            }
        }
        #endregion
        
        #region Functionality

        /// <summary>
        /// Character Damage Method.
        /// </summary>
        /// <param name="damage">Damage value.</param>
        /// <returns></returns>
        public virtual void OnHit(float damage)
        {
            CurrentHp -= damage;
        }

        private IEnumerator KillOnTimerCoroutine()
        {
            yield return new WaitForSeconds(DeathTimerValue);
            
            CharacterHandlerNotify?.Invoke(new CharacterArgs(true));

            Animator.enabled = false;
            ToggleRagdoll(true);

            yield return new WaitForSeconds(DelayDeathByTimer);
            
            CurrentHp = 0;
            _killOnTimerCoroutine = null;
           
        }

        private void ToggleRagdoll(bool enable)
        {
            if(rigidBodiesForRagdoll.Length == 0) return;

            foreach (var rb in rigidBodiesForRagdoll)
            {
                rb.isKinematic = !enable;
            }
        }
        
        #endregion
    }
}