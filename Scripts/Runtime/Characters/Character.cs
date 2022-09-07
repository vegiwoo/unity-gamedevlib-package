using GameDevLib.Helpers;
using GameDevLib.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    /// <summary>
    /// Essence of playable or non-playable character.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class Character : MonoBehaviour
    {
        #region Links
        [field:Header("Stats")] 
        [field:SerializeField] 
        public CharacterStats Stats { get; private set; }
        [field: SerializeField, ReadonlyField]
        public float CurrentHp { get; protected set; }
        [field: SerializeField, ReadonlyField]
        public float CurrentSpeed { get; set; }
        
        public Animator Animator { get; private set; }
        #endregion

        #region MonoBehaviour methods

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        
        protected void Start()
        {
            CurrentHp = Stats.MaxHp;
            CurrentSpeed = Stats.MoveSpeed;
        }

        protected void Update()
        {
            if (CurrentHp <= 0)
            {
                Destroy(gameObject);
            }
        }
        #endregion
        
        #region Functionality
        /// <summary>
        /// Character Damage Method.
        /// </summary>
        /// <param name="damage">Damage value.</param>
        /// <returns></returns>
        public abstract void OnHit(float damage);
        #endregion
    }
}