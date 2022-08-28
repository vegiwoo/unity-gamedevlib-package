// ReSharper disable once CheckNamespace
namespace GameDevLib
{
    /// <summary>
    /// Observer interface.
    /// </summary>
    public interface IObserver
    {
        void OnEventRaised(ISubject subject); 
    }
}