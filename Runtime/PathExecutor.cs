using EE.TalTech.IVAR.Robotics.ROSIndustrial.MoveItIntegration;
using RosMessageTypes.Moveit;

namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using Cysharp.Threading.Tasks;
    using MoveItIntegration;
    using ROSIndustrial;
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Makes the given robot follow a path of points provided by <see cref="PathPlanner"/> 
    /// </summary>
    public class PathExecutor : MonoBehaviour
    {
        #region Data

        public PathSelector pathSelector;

        public MoveItIKService ikService;
        public MoveItRobotMotionController robotController;

        /// <summary>
        /// Coordinate system that the path has to be executed in.
        /// </summary>
        public Transform pathCoordinateSpaceOrigin;

        [Header("Runtime values")]
        [SerializeField]
        [Restricted(RestrictedAttribute.Restrictions.ReadOnlyAlways)]
        private bool isExecuting = false;

        public bool IsExecuting
        {
            get => isExecuting;
            private set => isExecuting = value;
        }

        #endregion

        #region Methods

        public void ExecuteSelectedPathNoWait() { ExecuteSelectedPath().Forget(); }

        /// <summary>
        /// Executes the path currently selected in the connected PathPlanner.
        /// </summary>
        public async UniTaskVoid ExecuteSelectedPath()
        {
            IsExecuting = true;

            var pathPoses = pathSelector.SelectedPath.pointsPose;

            Debug.Log($"Starting executing '{pathSelector.SelectedPath}'...");

            for (var i = 0; i < pathPoses.Count; i++)
            {
                var pose = pathPoses[i];

                // Log
                Debug.Log($"Executing motion to pose: {pose}");

                // Convert pose back to world space before using it for IK
                var ikPose = new Pose
                {
                    position = pathCoordinateSpaceOrigin.TransformPoint(pose.position),
                    rotation = pathCoordinateSpaceOrigin.rotation * pose.rotation
                };

                // Calculate IK
                var ikSolution = await ikService.ComputeIK(ikPose);

                // Handle response
                if (ikSolution.error_code.val != MoveItErrorCodesMsg.SUCCESS)
                {
                    var error = new MoveItErrorCode(ikSolution.error_code.val);
                    Debug.LogError($"Path execution failed ('{pathSelector.SelectedPath}'): Error when computing IK. MoveIt error code {error.intValue} ({error})");
                    return;
                }

                string ikResults = "";
                foreach (double jointAngle in ikSolution.solution.joint_state.position) { ikResults += $"    {jointAngle}\n"; }

                //Debug.Log($"Received IK solution in {(Time.time - timer)} s:\n{ikResults}");

                // Move to solution
                Debug.Log($"Starting motion to point {i + 1}/{pathPoses.Count}...");

                string[] jointNames = ikSolution.solution.joint_state.name;
                double[] jointPositionsRos = ikSolution.solution.joint_state.position;
                double[] jointPositionsUnity = RobotJointPositionsConversionUtility.RosToUnity(robotController.robotKinematics, jointNames, jointPositionsRos);
                await robotController.Move(jointNames, jointPositionsUnity);

                Debug.Log($"Move to point {i + 1}/{pathPoses.Count}.");
            }

            Debug.Log($"Finished executing path '{pathSelector.SelectedPath}'!");

            IsExecuting = false;
        }

        #endregion
    }
}