using System.Collections.Generic;
using UnityEngine;
using GameDevLib.Args;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    /// <summary>
    /// Event receiving input from user.
    /// </summary>
    [CreateAssetMenu(fileName = "InputManagerEvent", menuName = "SO Events/InputManagerEvent", order = 0)]
    public class InputManagerEvent : ScriptableObject, ISubject<InputManagerArgs>
    {
        private readonly List<IObserver<InputManagerArgs>> _observers = new ();

        public void Attach(IObserver<InputManagerArgs> observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver<InputManagerArgs> observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(InputManagerArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}