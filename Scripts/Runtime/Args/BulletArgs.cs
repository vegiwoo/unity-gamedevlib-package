using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public sealed class BulletArgs
    {
        #region Properties
        public string TargetTag { get; }
        public Transform PointOfShoot { get; }
        public Vector3 TargetPosition { get; }
        public float Speed { get; }
        public float Range { get; }
        public int Damage { get; }
        #endregion
        
        #region Constructors

        public BulletArgs(string targetTag, Transform pointOfShoot, Vector3 targetPosition, float speed, float range, int damage)
        {
            TargetTag = targetTag;
            PointOfShoot = pointOfShoot;
            TargetPosition = targetPosition;
            Speed = speed;
            Range = range;
            Damage = damage;
        }
        #endregion
    }
}