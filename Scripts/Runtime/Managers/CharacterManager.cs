using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Args;
using GameDevLib.Characters;
using GameDevLib.Events;
using GameDevLib.Routes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Managers
{
    public class CharacterManager : MonoBehaviour
    {
        #region Links
        
        [SerializeField] private Character characterPrefab;
        [SerializeField] private CharacterRoute[] routes;
        [SerializeField] private CharacterManagerEvent characterManagerEvent;
        
        #endregion

        #region Constants and variables
        
        /// <summary>
        /// Inner collection of characters.
        /// </summary>
        private Dictionary<Character, CharacterMovement> _characters;
        
        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            _characters = new  Dictionary<Character, CharacterMovement>(32);

            foreach (var route in routes)
            {
                route.Init(characterManagerEvent);
            }

            StartCoroutine(CheckingCharactersOnRoutes());
        }

        #endregion
        
        #region Functionality
        
        /// <summary>
        /// Checks number of active characters on route.
        /// </summary>
        private IEnumerator CheckingCharactersOnRoutes()
        {
            while (routes.Length > 0)
            {
                yield return StartCoroutine(RemoveMissingCharacters());
                
                foreach (var route in routes)
                {
                    var charactersOnRouteCount = _characters
                        .Count(ch => ch.Value.Route.stats.RouteName == route.stats.RouteName);
                        
                    if (charactersOnRouteCount == route.stats.MaхNumberCharacters || route.SpawnTimer!= 0) continue;

                    var spawnPoint = route.FirstWaypoint;
                    var newCharacter = Instantiate(characterPrefab, spawnPoint, Quaternion.identity);

                    if (newCharacter.TryGetComponent<CharacterMovement>(out var newCharacterMovement))
                    {
                        newCharacterMovement.Route = route;
                        _characters.Add(newCharacter, newCharacterMovement);

                        var args = new RouteArgs(route.stats.RouteName, 1, 0);
                        characterManagerEvent.Notify(args);
                    }
                    else
                    {
                        Destroy(newCharacter);
                    }
                }

                yield return null;
            }
        }
        
        
        /// <summary>
        /// Removes missing characters from routes.
        /// </summary>
        private IEnumerator RemoveMissingCharacters()
        {
            foreach (var (character, characterMovement) in _characters.ToList())
            {
                if(character != null && character.CurrentHp > 0) continue;

                _characters.Remove(character);
                Destroy(character.gameObject);

                var args = new RouteArgs(characterMovement.Route.stats.RouteName, 0, 1);
                characterManagerEvent.Notify(args);
            }

            yield return null;
        }

        #endregion
    }
}