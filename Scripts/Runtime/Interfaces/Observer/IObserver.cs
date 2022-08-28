// ReSharper disable once CheckNamespace
namespace GameDevLib
{
    /// <summary>
    /// Observer interface.
    /// </summary>
    public interface IObserver<T>
    {
        void OnEventRaised(ISubject<T> subject); 
    }
}