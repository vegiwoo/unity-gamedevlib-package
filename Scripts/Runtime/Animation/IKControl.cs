using System;
using Cinemachine.Utility;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Animations
{
    /// <summary>
    /// Monitors inverse kinematics states.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour, IAnimatorParametersWorkable
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

        [field: SerializeField] 
        private LayerMask[] LayersOfFootPlacement { get; set; }
        
        [field: Space(4)]
        [field: SerializeField]
        private Transform TargetForHead { get; set; }
        [field: SerializeField, Tooltip("in meters")]
        private float HeadTrackingDistance  { get; set; } = 2.0f;
        
        private Animator _animator;
        private int _leftFootWalking;
        private int _rightFootWalking;

        #endregion
        
        #region Constants and variables
        private const int MaxWeight = 1;
        private const int MinWeight = 0;
        
        // Weight on IK foots controllers from Curves on animation clips.
        [field: SerializeField, ReadonlyField]
        private float LeftFootCurrentWeight { get; set; }
        
        [field: SerializeField, ReadonlyField]
        private float RightFootCurrentWeight { get; set; }

        // Raycast hit foot positions
        private Vector3 LeftFootPosition, RightFootPosition; 
        
        // Foot transforms 
        private Transform LeftFootTransform, RightFootTransform;
        
        // Difference of model and controllers
        public Vector3 LeftFootOffset, RightFootOffset;
        
        
        #endregion
        
        #region MonoBehaviour methods
        private void Start()
        {
            _animator = GetComponent<Animator>();
            
            AssignAnimationIDs();
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
            
            // Foots 
            // Get weights
            LeftFootCurrentWeight = _animator.GetFloat(_leftFootWalking);
            RightFootCurrentWeight = _animator.GetFloat(_rightFootWalking);
            
            // Set weights for IK controllers
            SetIKWeightPositionRotation(AvatarIKGoal.LeftFoot, LeftFootCurrentWeight);
            SetIKWeightPositionRotation(AvatarIKGoal.RightFoot, RightFootCurrentWeight);
            
            // Raycasting for foots
            IKRaycasting(AvatarIKGoal.LeftFoot);
            IKRaycasting(AvatarIKGoal.RightFoot);
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
        private void SetIKWeightPositionRotation(AvatarIKGoal goal, float? weight = null, Vector3? position = null, Quaternion? rotation = null)
        {
            if (weight != null)
            {
                _animator.SetIKPositionWeight(goal, weight.Value);
                _animator.SetIKRotationWeight(goal, weight.Value);
            }

            if (position != null)
            {
                _animator.SetIKPosition(goal, position.Value);
            }
            
            if (rotation != null)
            {
                _animator.SetIKRotation(goal, rotation.Value);
            }
        }

        private void IKRaycasting(AvatarIKGoal goal)
        {
            const float distance = 2.0f;
            var position = _animator.GetIKPosition(goal);
            
            if (goal == AvatarIKGoal.LeftFoot)
            {
                LeftFootPosition = position;
            }
            else if (goal == AvatarIKGoal.RightFoot)
            {
                RightFootPosition = position;
            }

            // Raycast from foot to ground, given the target location layer
            foreach (var layer in LayersOfFootPlacement)
            {
                if (!Physics.Raycast(position + Vector3.up, Vector3.down, out var hit, distance, layer)) continue;
                
                // Draw construction lines in editor
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow);
                    
                if (goal == AvatarIKGoal.LeftFoot)
                {
                    SetIKWeightPositionRotation(
                        goal, 
                        null, 
                        hit.point + LeftFootOffset, 
                        Quaternion.LookRotation(Vector3.ProjectOnPlane(LeftFootTransform.forward, hit.normal), hit.normal));
                    
                    LeftFootPosition = hit.point;
                }
                else if (goal == AvatarIKGoal.RightFoot)
                {
                    SetIKWeightPositionRotation(
                        goal, 
                        null, 
                        hit.point + RightFootOffset, 
                        Quaternion.LookRotation(Vector3.ProjectOnPlane(RightFootTransform.forward, hit.normal), hit.normal));
                    RightFootPosition = hit.point;
                }
            }
        }
        public void AssignAnimationIDs() 
        {
            _leftFootWalking = Animator.StringToHash("Left_Leg_Walking");
            _rightFootWalking = Animator.StringToHash("Right_Leg_Walking");
        }
        #endregion
    }
}