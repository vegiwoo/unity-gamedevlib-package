// ReSharper disable once CheckNamespace
namespace GameDevLib.Units.Interfaces
{
    /// <summary>
    /// Specifies ability to take damage.
    /// </summary>
    public interface ITakingDamagable
    {
        #region Properties
        
        public int MaxHipPoints { get; protected set; }
        public int CurrentHipPoints { get; protected set; }
        public bool IsInvulnerable { get; protected set; }
        public bool IsDead { get; protected set; }
        
        #endregion
        
        #region Functionality

        protected void SetMaxHipPoints(int value)
        {
            MaxHipPoints = value;
        }
        
        protected void SetInvulnerable(bool value)
        {
            IsInvulnerable = value;
        }

        protected void SetHitPoints(int value, bool increase)
        {
            switch (increase)
            {
                case true:
                    CurrentHipPoints = CurrentHipPoints + value <= MaxHipPoints ? 
                        CurrentHipPoints += value : 
                        MaxHipPoints;
                    break;
                case false:
                    if (!IsInvulnerable)
                    {
                        CurrentHipPoints = CurrentHipPoints - value > 0 ? 
                            CurrentHipPoints -= value : 
                            0;
                    
                        if (CurrentHipPoints == 0)
                        {
                            IsDead = true;
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}