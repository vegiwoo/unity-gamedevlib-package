using System;
using GameDevLib.Helpers;
using GameDevLib.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    /// <summary>
    /// Essence of playable or non-playable character.
    /// </summary>
    public abstract class Character : MonoBehaviour
    {
        #region Links
        [field:Header("Stats")] 
        [field:SerializeField] 
        public CharacterStats Stats { get; private set; }
        [field: SerializeField, ReadonlyField, Tooltip("Current hit points.")]
        public float CurrentHp { get; protected set; }
        [field: SerializeField, ReadonlyField, Tooltip("Character's current movement speed")]
        protected float CurrentSpeed { get;  set; }
        #endregion

        #region MonoBehaviour methods

        protected void Start()
        {
            CurrentHp = Stats.MaxHp;
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