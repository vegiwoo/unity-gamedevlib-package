// ReSharper disable once CheckNamespace
namespace GameDevLib
{
    /// <summary>
    /// Publisher interface.
    /// </summary>
    public interface ISubject
    {
        /// <summary>  
        /// Attaches an observer to a publisher.    
        /// </summary>    
        /// <param name="observer">Concrete observer.</param>    
        void Attach(IObserver observer);  
    
        /// <summary>  
        /// Detach observer from publisher.    
        /// </summary>    
        /// <param name="observer">Concrete observer.</param>    
        void Detach(IObserver observer);  
  
        /// <summary>  
        /// Notifies all observers of the event.   
        /// </summary>    
        void Notify(); 
    }
}