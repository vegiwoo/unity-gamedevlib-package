
// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
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