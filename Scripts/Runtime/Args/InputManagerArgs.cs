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
        public bool Lighting { get;}
        public bool? Firing { get;}
        #endregion

        #region Constructors
        public InputManagerArgs(bool lighting, Vector2? moving = null, bool? running = null, bool? jumping = null, bool? aiming = null, bool? firing = null)
        {
            Lighting = lighting;
            Moving = moving;
            Running = running;
            Jumping = jumping;
            Aiming = aiming;
            Firing = firing;
        }
        #endregion 
    }
}