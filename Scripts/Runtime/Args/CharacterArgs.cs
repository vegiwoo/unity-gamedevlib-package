using System;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public sealed class CharacterArgs : EventArgs
    {
        public bool DiedByTimer { get; }

        public CharacterArgs(bool diedByTimer)
        {
            DiedByTimer = diedByTimer;
        }
    }
}