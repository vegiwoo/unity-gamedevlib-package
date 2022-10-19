using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public sealed class InputManagerArgs : EventArgs
    {
        #region Properties
        
        public Vector2? Moving { get;}
        public bool? Running { get;}
        public bool? Jumping { get; set; }
        public bool? Aiming { get;}
        public bool Lighting { get;}
        public bool Firing { get;}
        
        #endregion

        #region Constructors
        public InputManagerArgs(bool lighting, bool firing, Vector2? moving = null, bool? running = null, bool? jumping = null, bool? aiming = null)
        {
            Lighting = lighting;
            Firing = firing;
            Moving = moving;
            Running = running;
            Jumping = jumping;
            Aiming = aiming;
        }
        #endregion 
    }
}