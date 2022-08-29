
// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public class InputManagerArgs
    {
        public bool? Aiming { get; private set; }

        public InputManagerArgs(bool? aiming = null)
        {
            Aiming = aiming;
        }
    }

}

