using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public sealed class InputManagerArgs
    {
        #region Properties
        public Vector2? Moving { get;}
        public bool? Running { get;}
        public bool? Jumping { get;}
        public bool? Aiming { get;}
        public bool? Lighting { get;}
        #endregion

        #region Constructors
        public InputManagerArgs(Vector2? moving = null, bool? running = null, bool? jumping = null, bool? aiming = null, bool? lighting = null)
        {
            Moving = moving;
            Running = running;
            Jumping = jumping;
            Aiming = aiming;
            Lighting = lighting;
        }
        #endregion 
    }
}