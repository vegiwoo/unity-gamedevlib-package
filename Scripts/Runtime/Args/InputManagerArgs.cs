using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public class InputManagerArgs
    {
        public bool? Aiming { get;}
        public Vector2? Moving { get;}
        public bool? Running { get;}
        public bool? Jumping { get;}

        public InputManagerArgs(bool? aiming = null, Vector2? moving = null, bool? running = null, bool? jumping = null)
        {
            Aiming = aiming;
            Moving = moving;
            Running = running;
            Jumping = jumping;
        }
    }
}

