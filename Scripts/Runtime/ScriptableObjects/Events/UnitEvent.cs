using System.Collections.Generic;
using GameDevLib.Args;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "UnitEvent", menuName = "SO Events/UnitEvent", order = 2)]
    public class UnitEvent : ScriptableObject, ISubject<UnitArgs>
    {
        private readonly List<IObserver<UnitArgs>> _observers = new();

        public void Attach(IObserver<UnitArgs> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
        }

        public void Detach(IObserver<UnitArgs> observer)
        {
            if (_observers.Contains(observer)) _observers.Remove(observer);
        }

        public void Notify(UnitArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}