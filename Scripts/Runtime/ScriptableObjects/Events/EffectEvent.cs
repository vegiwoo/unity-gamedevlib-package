using System;
using GameDevLib.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "EffectEvent", menuName = "SO Events/EffectEvent", order = 3)]
    public class EffectEvent : GameEvent<EventArgs> { }
}