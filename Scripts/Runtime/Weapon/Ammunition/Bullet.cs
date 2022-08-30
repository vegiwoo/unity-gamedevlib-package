using System;
using System.Collections;
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
        private Light _pointLight;
        #endregion
        
        #region Monobehavior methods

        protected override void Start()
        {
            base.Start();
 
            _pointLight = gameObject.AddComponent<Light>();
            _pointLight.type = LightType.Point;
            _pointLight.color = Color.yellow;
            _pointLight.intensity = 10;
        }

        private void FixedUpdate()
        {
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

            StartCoroutine(OnCollisionCoroutine());
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

        private IEnumerator OnCollisionCoroutine()
        {
            collisionEffect.Play();
            yield return new WaitWhile(() => collisionEffect.isPlaying);
            yield return new WaitForSeconds(0.05f);
            Destroy(gameObject);
        }
        
        
        #endregion
    }
}