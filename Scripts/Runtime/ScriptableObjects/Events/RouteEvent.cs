using GameDevLib.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "RoureEvent", menuName = "GameDevLib/Events/RoureEvent",
        order = 1)]
    public class RouteEvent : GameEvent<RouteArgs> { }
}