using System;
using GameDevLib.Stats;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Units.Interfaces
{
    /// <summary>
    /// Specifies that unit is moving.
    /// </summary>
    public interface IMovable
    {
        #region Properties 
        public MovingStats MovingStats { get; set; }
        float CurrentAccelerationFactor { get; set; }
        public float CurrentVelocity { get; protected set; }
        public bool IsSpeedUp { get; protected set; }
        public bool IsSpeedDown { get; protected set; }
        public float ComparisonTolerance => 0.1f;

        #endregion
        
        #region Functionality
        
        protected void SetSpeed(float? multiplier, bool? increase, bool? isEffectCancelled)
        {
            if (isEffectCancelled.HasValue)
            {
                CurrentAccelerationFactor = MovingStats.AccelerationFactor;
            } 
            else if (multiplier.HasValue && increase.HasValue)
            {
                switch (increase)
                {
                    case true:
                        if (CurrentAccelerationFactor < MovingStats.AccelerationFactor || Math.Abs(CurrentAccelerationFactor - MovingStats.AccelerationFactor) < ComparisonTolerance)
                        {
                            CurrentAccelerationFactor = MovingStats.AccelerationFactor * multiplier.Value;
                        }
                        break;
                    case false:
                        if (CurrentAccelerationFactor > MovingStats.AccelerationFactor || Math.Abs(CurrentAccelerationFactor - MovingStats.AccelerationFactor) < ComparisonTolerance)
                        {
                            CurrentAccelerationFactor = MovingStats.AccelerationFactor / multiplier.Value;
                        }
                        break;
                }
            }
            
            IsSpeedUp = CurrentAccelerationFactor > MovingStats.AccelerationFactor;
            IsSpeedDown = CurrentAccelerationFactor < MovingStats.AccelerationFactor;
        }

        public void SetVelocity(float velocity)
        {
            CurrentVelocity = velocity;
        }
        
        public void Move();

        #endregion
    }
}