using System.Collections;
using GameDevLib.Animations;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Stats;
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
        [field: SerializeField, Tooltip("What layers the character uses as ground")]
        public LayerMask[] GroundLayers { get; set; }

        [field: SerializeField, Tooltip("Physical bodies in a ragdoll object")] 
        private Rigidbody[] rigidBodiesForRagdoll;
        
        [field: Header("Kill by timer")]
        [field: SerializeField] 
        public bool IsDeathByTimer { get; set; }
        [field: SerializeField, Tooltip("in sec")]
        public float DeathTimerValue { get; set; } = 3f;

        [field: SerializeField, Tooltip("Delay before destruction after death by timer")] 
        private float DelayDeathByTimer { get; set; } = 5f;

        [field: Header("Stats")]
        [field:SerializeField] public CharacterStats CharacterStats { get; set; }
        
        [field: Header("Events")]
        [field:SerializeField] public UnitEvent UnitEvent { get; set; }
        
        #endregion
        
        #region Properties
        [field: SerializeField, ReadonlyField] public float CurrentHp { get; protected set; }
        [field: SerializeField, ReadonlyField] public float CurrentSpeed { get; set; }
        
        public Animator Animator { get; private set; }
        
        #endregion
        
        #region Fields
        
        private Coroutine _killOnTimerCoroutine;
        
        public delegate void CharacterHandler(CharacterArgs args);  
        public event CharacterHandler? CharacterHandlerNotify;
        
        #endregion
        
        #region MonoBehaviour methods

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        
        protected virtual void Start()
        {
            ToggleRagdoll(false);
            
            CurrentHp = CharacterStats.MaxHp;
            CurrentSpeed = CharacterStats.MoveSpeed;
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
            CurrentHp = CurrentHp - damage > 0 ? CurrentHp - damage : 0;
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

        protected virtual void Notify() {}

        #endregion
    }
}