
// ReSharper disable once CheckNamespace
namespace GameDevLib.Args
{
    public class RouteArgs 
    {
        public string RouteName { get; }
        public int NumberCharactersCreated { get; }
        public int NumberCharactersMissing { get; }

        public RouteArgs(string routeName, int numberCharactersCreated, int numberCharactersMissing)
        {
            RouteName = routeName;
            NumberCharactersCreated = numberCharactersCreated;
            NumberCharactersMissing = numberCharactersMissing;
        }
    }
}