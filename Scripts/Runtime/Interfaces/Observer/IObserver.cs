// ReSharper disable once CheckNamespace
namespace GameDevLib.Interfaces
{
    /// <summary>
    /// Observer interface.
    /// </summary>
    public interface IObserver<T>
    {
        /// <summary>
        /// Called by publisher when an event occurs.
        /// </summary>
        /// <param name="subject">Publisher who published event.</param>
        /// <param name="args">Arguments passed with event.</param>
        void OnEventRaised(ISubject<T> subject, T args); 
    }
}