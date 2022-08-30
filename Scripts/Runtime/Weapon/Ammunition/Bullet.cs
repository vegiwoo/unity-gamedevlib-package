using GameDevLib.Args;
using GameDevLib.Characters;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Ammunition
{
    /// <summary>
    /// Represents item of projectile.
    /// </summary>
    public class Bullet : Ammunition
    {
        #region Constants, variables and properties

        private BulletArgs _bulletArgs;
        #endregion
        
        #region Monobehavior methods

        private void FixedUpdate()
        {
            //if(_bulletArgs == null) return;
            if (Vector3.Distance(_bulletArgs.PointOfShoot.position, transform.position) > _bulletArgs.Range)
            {
                Destroy(gameObject);
            }

            AmmunitionRigidbody.velocity =
                (_bulletArgs.TargetPosition - _bulletArgs.PointOfShoot.position) * _bulletArgs.Speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Character>(out var character) &&
                character.gameObject.CompareTag(_bulletArgs.TargetTag))
            {
                character.OnHit(_bulletArgs.Damage);
            }

            if (collisionEffect != null)
            {
                collisionEffect.Play();
            }
            
            Destroy(gameObject);
        }

        #endregion
        
        #region Functionality
        /// <summary>
        /// Assigning parameters for a bullet from a weapon.
        /// </summary>
        public void Init(BulletArgs bulletArgs)
        {
            _bulletArgs = bulletArgs;
        }
        #endregion
    }
}