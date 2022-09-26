using UnityEngine;
using GameDevLib.Args;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Events
{
    [CreateAssetMenu(fileName = "InputEvent", menuName = "GameDevLib/InputEvent", order = 0)]
    public class InputEvent : GameEvent<InputManagerArgs> { }
}