using GameDevLib.Args;
using GameDevLib.Interfaces;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    public class CharacterRigging : MonoBehaviour, IObserver<InputManagerArgs>
    {
        [SerializeField] 
        private MultiParentConstraint stowedPosWeaponIK;

        private const float StowedPositionWeaponIKNormalWeight = 0.95f;
        
        private void Start()
        {
            if (stowedPosWeaponIK)
            {
                stowedPosWeaponIK.weight = StowedPositionWeaponIKNormalWeight;
            }
        }

        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            if (args.Aiming.HasValue && stowedPosWeaponIK)
            {
                stowedPosWeaponIK.weight = args.Aiming.Value ? 0 : StowedPositionWeaponIKNormalWeight;
            } 
            
        }
    }
}