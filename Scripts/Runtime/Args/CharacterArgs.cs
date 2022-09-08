
// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    public sealed class CharacterArgs
    {
        public bool DiedByTimer { get; }

        public CharacterArgs(bool diedByTimer)
        {
            DiedByTimer = diedByTimer;
        }
    }
}