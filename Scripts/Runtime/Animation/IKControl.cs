using GameDevLib.Characters;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Animations
{
    /// <summary>
    /// Monitors inverse kinematics states.
    /// </summary>
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

        [field: Space(4)]
        [field: SerializeField]
        public Transform TargetForHead { get; set; }
        [field: SerializeField, Tooltip("in meters")]
        public float HeadTrackingDistance  { get; set; } = 2.0f;
        
        private Character _character;
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
        private Vector3 _leftFootPosition, _rightFootPosition; 
        
        // Foot transforms 
        public Transform _leftFootTransform, _rightFootTransform;
        
        // Difference of model and controllers
        public Vector3 leftFootOffset, rightFootOffset;
        
        #endregion
        
        #region MonoBehaviour methods

        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            AssignAnimationIDs();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!_character.Animator || !ikActive) return;

            // Setting look target for head
            if (TargetForHead != null)
            {
                var isTurnHead = Vector3.Distance(transform.position, TargetForHead.position) < HeadTrackingDistance;
                if (isTurnHead)
                {
                    _character.Animator.SetLookAtWeight(MaxWeight);
                    _character.Animator.SetLookAtPosition(TargetForHead.position);
                }
                else
                {
                    _character.Animator.SetLookAtWeight(MinWeight);
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
            LeftFootCurrentWeight = _character.Animator.GetFloat(_leftFootWalking);
            RightFootCurrentWeight = _character.Animator.GetFloat(_rightFootWalking);
            
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
                _character.Animator.SetIKPositionWeight(goal, weight.Value);
                _character.Animator.SetIKRotationWeight(goal, weight.Value);
            }

            if (position != null)
            {
                _character.Animator.SetIKPosition(goal, position.Value);
            }
            
            if (rotation != null)
            {
                _character.Animator.SetIKRotation(goal, rotation.Value);
            }
        }

        private void IKRaycasting(AvatarIKGoal goal)
        {
            if(_character.GroundLayers == null || _character.GroundLayers.Length == 0) return;
            
            const float distance = 2.0f;
            var position = _character.Animator.GetIKPosition(goal);
            
            if (goal == AvatarIKGoal.LeftFoot)
            {
                _leftFootPosition = position;
            }
            else if (goal == AvatarIKGoal.RightFoot)
            {
                _rightFootPosition = position;
            }

            // Raycast from foot to ground, given the target location layer
            foreach (var layer in _character.GroundLayers)
            {
                if (!Physics.Raycast(position + Vector3.up, Vector3.down, out var hit, distance, layer)) continue;
                
                // Draw construction lines in editor
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow);

                switch (goal)
                {
                    case AvatarIKGoal.LeftFoot:
                        _character.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + leftFootOffset);
                        _character.Animator.SetIKRotation(AvatarIKGoal.LeftFoot,  Quaternion.LookRotation(Vector3.ProjectOnPlane(_leftFootTransform.forward, hit.normal), hit.normal));
                        _leftFootPosition = hit.point;
                        break;
                    case AvatarIKGoal.RightFoot:
                        _character.Animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + rightFootOffset);
                        _character.Animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(_rightFootTransform.forward, hit.normal), hit.normal));
                        _rightFootPosition = hit.point;
                        break;
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