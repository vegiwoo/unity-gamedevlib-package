using System.Collections.Generic;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    public abstract class GameEvent<T> : ScriptableObject, ISubject<T>
    {
        private readonly List<IObserver<T>> _observers = new();
        
        public void Attach(IObserver<T> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
        }

        public void Detach(IObserver<T> observer)
        {
            if (_observers.Contains(observer)) _observers.Remove(observer);
        }

        public void Notify(T args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}