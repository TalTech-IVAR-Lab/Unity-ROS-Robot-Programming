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
        public RosIndustrialRobotMotionController robotController;

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

            var path = pathSelector.SelectedPath.pointsPose;

            foreach (var point in path)
            {
                // Calculate IK
                var ikSolution = await ikService.ComputeIK(point);
                string[] jointNames = ikSolution.solution.joint_state.name;
                double[] jointPositions = ikSolution.solution.joint_state.position;

                // Move to solution
                await robotController.Move(jointNames, jointPositions);
            }

            IsExecuting = false;
        }

        #endregion
    }
}