using System.Collections.Generic;
using GameDevLib.Args;
using GameDevLib.Interfaces;
using GameDevLib.Managers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "CharacterManagerEvent", menuName = "SO Events/CharacterManagerEvent", order = 1)]
    public class CharacterManagerEvent : ScriptableObject, ISubject<RouteArgs>
    {
        private readonly List<IObserver<RouteArgs>> _observers = new ();
        
        public void Attach(IObserver<RouteArgs> observer)
        {
            if (!_observers.Contains(observer))  _observers.Add(observer);
        }

        public void Detach(IObserver<RouteArgs> observer)
        {
            if (_observers.Contains(observer))  _observers.Remove(observer);
        }

        public void Notify(RouteArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}