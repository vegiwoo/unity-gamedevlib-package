using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using GameDevLib.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Managers
{
    public abstract class GameManager : MonoBehaviour, IObserver<UnitArgs>
    {
        #region Links
        [field: SerializeField] public GameStats GameStats { get; set; }
        [field: SerializeField] public UnitEvent PlayerEvent { get; set; }
        #endregion
        
        #region MonoBehaviour methods

        protected virtual void OnEnable()
        {
            PlayerEvent.Attach(this);
        }

        protected virtual void OnDisable()
        {
            PlayerEvent.Detach(this);
        }

        #endregion

        #region Functionality

        // In this method make logic of keeping score of game, condition of victory or loss and end game. 
        public abstract void OnEventRaised(ISubject<UnitArgs> subject, UnitArgs args);

        #endregion
    }
}