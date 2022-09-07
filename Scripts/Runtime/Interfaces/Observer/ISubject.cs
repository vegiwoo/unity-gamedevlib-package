// ReSharper disable once CheckNamespace
namespace GameDevLib.Interfaces
{
    /// <summary>
    /// Publisher interface.
    /// </summary>
    public interface ISubject<T>
    {
        /// <summary>  
        /// Attaches an observer to a publisher.    
        /// </summary>    
        /// <param name="observer">Concrete observer.</param>    
        void Attach(IObserver<T> observer);  
    
        /// <summary>  
        /// Detach observer from publisher.    
        /// </summary>    
        /// <param name="observer">Concrete observer.</param>    
        void Detach(IObserver<T> observer);  
  
        /// <summary>  
        /// Notifies all observers of the event.   
        /// </summary>    
        void Notify(T args); 
    }
}