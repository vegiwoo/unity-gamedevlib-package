using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Animations
{
    /// <summary>
    /// Monitors inverse kinematics states.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour
    {
        #region Links
        [field: Header("Common")] 
        [field: SerializeField]
        private bool ikActive;
        
        [field: Header("Targets")] 
        [field: SerializeField]
        private Transform TargetForLeftHand { get; set; }
        [field: SerializeField]
        private Transform TargetForRightHand { get; set; }
        [field: SerializeField, Tooltip("in meters")] 
        private float HandContactDistance { get; set; } = 0.5f;
        
        [field: Space(4)]
        [field: SerializeField]
        private Transform TargetForHead { get; set; }
        [field: SerializeField, Tooltip("in meters")]
        private float HeadTrackingDistance  { get; set; } = 2.0f;


        private Animator _animator;
        #endregion
        
        #region Constants and variables
        private const int MaxWeight = 1;
        private const int MinWeight = 0;

        private float state = 0;
        private float elapsedTime = 0;
        private float timeReaction = 0.5f;
        #endregion
        
        #region MonoBehaviour methods
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!_animator || !ikActive) return;
            
            // Setting look target for head
            if (TargetForHead != null)
            {
                var isTurnHead = Vector3.Distance(transform.position, TargetForHead.position) < HeadTrackingDistance;
                if (isTurnHead)
                {
                    _animator.SetLookAtWeight(MaxWeight);
                    _animator.SetLookAtPosition(TargetForHead.position);
                }
                else
                {
                    _animator.SetLookAtWeight(MinWeight);
                }
            }
            
            var position = transform.position;

            // Setting target for left hand and put it in position
            if (TargetForLeftHand != null)
            {
                var leftHandTp = TargetForLeftHand.position;
                var isTurn = Vector3.Distance(position, leftHandTp) < HandContactDistance;
                SetIKWeightPositionRotation(AvatarIKGoal.LeftHand, isTurn ? MaxWeight : MinWeight, isTurn ? leftHandTp : null, isTurn ? TargetForLeftHand.rotation : null);
            }

            // Setting target for right hand and put it in position
            if (TargetForRightHand != null)
            {
                var rightHandTp = TargetForRightHand.position;
                var isTurn = Vector3.Distance(position, rightHandTp) < HandContactDistance;
                SetIKWeightPositionRotation(AvatarIKGoal.RightHand, isTurn ? MaxWeight : MinWeight, isTurn ? rightHandTp : null, isTurn ? TargetForRightHand.rotation : null);
            }
        }
        #endregion
        
        #region Functionality
        /// <summary>
        /// Sets weight, position and rotation of AvatarIKGoal target.
        /// </summary>
        /// <param name="goal">Target for setting parameters.</param>
        /// <param name="weight">Installed weight.</param>
        /// <param name="position">Installed position.</param>
        /// <param name="rotation">Installed rotation.</param>
        private void SetIKWeightPositionRotation(AvatarIKGoal goal, float weight, Vector3? position = null, Quaternion? rotation = null)
        {
            _animator.SetIKPositionWeight(goal, weight);
            _animator.SetIKRotationWeight(goal, weight);

            if (position == null || rotation == null) return;
            
            _animator.SetIKPosition(goal, position.Value);
            _animator.SetIKRotation(goal, rotation.Value);
        }
        #endregion
    }
}

