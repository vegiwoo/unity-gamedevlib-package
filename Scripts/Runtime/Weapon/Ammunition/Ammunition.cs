using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Ammunition
{
    /// <summary>
    /// Represents ammunition.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Ammunition : MonoBehaviour
    {
        #region Links
        [SerializeField] 
        protected ParticleSystem collisionEffect;
        protected Rigidbody AmmunitionRigidbody;
        #endregion
        
        #region MonoBehaviour methods

        protected virtual void Start()
        {
            AmmunitionRigidbody = GetComponent<Rigidbody>();
            if (collisionEffect != null)
            {
                collisionEffect.Stop();
            }
        }
        #endregion
    }
}