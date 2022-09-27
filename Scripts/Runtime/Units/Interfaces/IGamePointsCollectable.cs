// ReSharper disable once CheckNamespace
namespace GameDevLib.Units.Interfaces
{
    /// <summary>
    /// Specifies ability of a unit to receive game points.
    /// </summary>
    public interface IGamePointsCollectable
    {
        #region Properties 
        
        public int MaxGamePoints { get; protected set; }
        public int CurrentGamePoints { get; protected set; }
        public bool IsWin { get; protected set; }
        
        #endregion
        
        #region Functionality

        protected void InitGamePointsCollectable(int maxGamePoints)
        {
            MaxGamePoints = maxGamePoints;
        }

        protected void SetGamePoints(int value, bool increase)
        {
            switch (increase)
            {
                case true:
                    CurrentGamePoints = CurrentGamePoints + value <= MaxGamePoints ? 
                        CurrentGamePoints += value : 
                        MaxGamePoints;

                    if (CurrentGamePoints == MaxGamePoints)
                    {
                        IsWin = true;
                    }
                    break;
                case false:
                    CurrentGamePoints = CurrentGamePoints - value > 0 ? 
                        CurrentGamePoints -= value : 
                        0;
                    break;
            }
        }

        #endregion
    }
}