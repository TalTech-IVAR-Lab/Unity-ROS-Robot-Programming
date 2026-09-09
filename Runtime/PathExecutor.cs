namespace EE.TalTech.IVAR.Robotics.Programming.Paths
{
    using Cysharp.Threading.Tasks;
    using MoveItIntegration;
    using ROSIndustrial;
    using ROSIndustrial.MoveItIntegration;
    using RosMessageTypes.Moveit;
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

            try
            {
                var pathPoses = pathSelector.SelectedPath.pointsPose;

                Debug.Log($"Starting executing '{pathSelector.SelectedPath}'...");

                for (var i = 0; i < pathPoses.Count; i++)
                {
                    var pose = pathPoses[i];

                    // Convert pose back to world space before using it in the request
                    var worldPose = new Pose
                    {
                        position = pathCoordinateSpaceOrigin.TransformPoint(pose.position),
                        rotation = pathCoordinateSpaceOrigin.rotation * pose.rotation
                    };

                    Debug.Log($"PathExecutor point {i + 1}/{pathPoses.Count}: savedLocalPos={pose.position} savedLocalRot={pose.rotation.eulerAngles} " +
                              $"origin={pathCoordinateSpaceOrigin.name} originWorldPos={pathCoordinateSpaceOrigin.position} originWorldRot={pathCoordinateSpaceOrigin.rotation.eulerAngles} " +
                              $"executedWorldPos={worldPose.position} executedWorldRot={worldPose.rotation.eulerAngles}", this);

                    Debug.Log($"Starting motion to point {i + 1}/{pathPoses.Count}...");

                    bool motionSuccessful;

                    if (i == 0)
                    {
                        // The first move is an approach from the robot's arbitrary current pose to the start of the
                        // programmed path. It is not a visible path segment in the VR program, so do not force it to be
                        // a Cartesian LIN move. LIN is preserved for all programmed point-to-point segments below.
                        motionSuccessful = await robotController.MoveCartesianPtp(worldPose);
                    }
                    else
                    {
                        motionSuccessful = await robotController.MoveCartesian(worldPose);
                    }

                    if (!motionSuccessful)
                    {
                        Debug.LogError($"Motion to point {i + 1} failed. Please check the logs to identify the issue.\n" +
                                       $"Aborting the rest of the trajectory execution.");
                        break;
                    }

                    Debug.Log($"Moved to point {i + 1}/{pathPoses.Count}.");
                }

                Debug.Log($"Finished executing path '{pathSelector.SelectedPath}'.");
            }
            finally
            {
                IsExecuting = false;
            }
        }


        #endregion
    }
}