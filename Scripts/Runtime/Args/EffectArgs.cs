using System;
using GameDevLib.Interactions.Effects;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public class EffectArgs : EventArgs
    {
        public EffectType EffectType { get;}
        public EffectTargetType EffectTargetType  { get;}
        public float? Power  { get; private set; }
        public bool? Increase   { get; private set; }
        public bool? IsInvulnerable   { get; private set; }
        public bool? CancelEffect   { get; private set; }

        public EffectArgs(EffectType effectType, EffectTargetType effectTargetType)
        {
            EffectType = effectType;
            EffectTargetType = effectTargetType;
        }

        public void Init(float? power = null, bool? increase = null, bool? isInvulnerable = null,  bool? cancelEffect = null)
        {
            Power = power;
            Increase = increase;
            IsInvulnerable = isInvulnerable;
            CancelEffect = cancelEffect;
        }
    }
}