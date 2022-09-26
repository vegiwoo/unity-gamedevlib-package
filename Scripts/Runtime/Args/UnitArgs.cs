using System;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args {
    
    /// <summary>
    /// Arguments through which current state of unit is passed.
    /// </summary>
    public class UnitArgs : EventArgs
    {
        #region Properties
        // HP
        public float CurrentHp { get; }
        public bool IsUnitInvulnerable { get; }
        
        // Game points 
        public int GamePoints { get; }
        
        // Speed
        public double Velocity { get; }
        public bool IsSpeedUp { get; }
        public bool IsSpeedDown { get; }

        #endregion
        
        #region Constructors 
        
        public UnitArgs(float currentHp, bool isUnitInvulnerable,  float velocity, bool isSpeedUp, bool isSpeedDown, int gamePoints = 0)
        {
            CurrentHp = currentHp;
            IsUnitInvulnerable = isUnitInvulnerable;
            Velocity = Math.Round(velocity, 2);
            IsSpeedUp = isSpeedUp;
            IsSpeedDown = isSpeedDown;
            GamePoints = gamePoints;
        }
        
        #endregion
    }
}