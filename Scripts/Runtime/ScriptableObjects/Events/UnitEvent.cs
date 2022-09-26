using GameDevLib.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "UnitEvent", menuName = "SO Events/UnitEvent", order = 2)]
    public class UnitEvent : GameEvent<UnitArgs> { }
}